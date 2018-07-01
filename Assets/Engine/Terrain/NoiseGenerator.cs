using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class NoiseGenerator
{
	 
	private int _width;
	private int _height;
	private int _octaves;
	private int _seed;

	private float[] _baseTexture;
	private float[] _finalNoise;

	private float _step = 0.5f;
	private float _amplitude = 1.0f;
	private float _frequencyMod = 4.0f;

	private bool[] _mapBlueprint;

	private bool _createBottom;

	private float _initialHeight = 0.1f;
	private bool _generatePlayableHeight = true;

	private float _playableHeightPercent = 0.05f;
	private float _bottomPlanePercent = 0.025f;

	private float _playableHeight = 0.0f;
	private float _bottomPlane = -1.0f;

	private float _peakHeight = 0.0f;
	private float _bottomHeight = Mathf.Infinity;

	public NoiseGenerator (int resolution, int octaves, bool createBottom, bool[] blueprint)
		: this (resolution, resolution, octaves, createBottom, blueprint, (int)(Random.value * 100000.0f))
	{
	}

	public NoiseGenerator (int resolution, int octaves, bool createBottom, bool[] blueprint, int seed)
		: this (resolution, resolution, octaves, createBottom, blueprint, seed)
	{
	}

	public NoiseGenerator (int width, int height, int octaves, bool createBottom, bool[] blueprint, int seed)
	{
		_width = width;
		_height = height;
		_mapBlueprint = blueprint;

		_octaves = Mathf.Max (octaves, 0);
		_createBottom = createBottom;
		_seed = seed;

		_baseTexture = new float[_width * _height];
		_finalNoise = new float[_width * _height];
	}

	public void setInitialHeight (float height)
	{
		_initialHeight = height;
	}

	public void generatePlayableHeight (bool val)
	{
		_generatePlayableHeight = val;
	}

	public void setFrequencyMod (float mod)
	{
		_frequencyMod = mod;
	}

	public void setStep (float step)
	{
		_step = step;
	}

	public void setAmplitude (float ampl)
	{
		_amplitude = ampl;
	}

	public void setCreateBottom (bool val)
	{
		_createBottom = val;
	}

	public void setPlayableHeightPercent (float val)
	{
		_playableHeightPercent = val;
	}

	public void setBottomHeightPercent (float val)
	{
		_bottomPlanePercent = val;
	}

	public void generateRandomNoise ()
	{
		int end = _width * _height;

		Random.InitState (_seed);

		for (int i = 0; i < end; i++) {
			if (_mapBlueprint [i])
				_baseTexture [i] = _initialHeight;
			else
				_baseTexture [i] = Random.value;
		}
	}

	private float [] generateOctave (int octaveIndex)
	{
		float[] result = new float[_width * _height];

		float period = 1 << octaveIndex;
		period /= _frequencyMod;
		float frequency = 1.0f / period;

		int x0y0, x1y0;
		int x0y1, x1y1;
		float hBlend, vBlend;

		float a, b;

		for (int i = 0; i < _width; i++) {
			x0y0 = (int)((int)(i / period) * period);
			x1y0 = (int)((int)(x0y0 + period) % _width);
			hBlend = (i - x0y0) * frequency;


			for (int j = 0; j < _height; j++) {
				int index = (i * _width + j);

				if (_mapBlueprint [index]) {
					result [index] = _baseTexture [index];
				} else {
					x0y1 = (int)((int)(j / period) * period);
					x1y1 = (int)((int)(x0y1 + period) % _height);
					vBlend = (j - x0y1) * frequency;

					a = Mathf.Lerp (_baseTexture [x0y0 * _width + x0y1], _baseTexture [x1y0 * _width + x0y1], hBlend);
					b = Mathf.Lerp (_baseTexture [x0y0 * _width + x1y1], _baseTexture [x1y0 * _width + x1y1], hBlend);

					result [index] = Mathf.Lerp (a, b, vBlend);
				}
			}
		}

		return result;
	}

	public void generateNoiseMap ()
	{
		generateRandomNoise ();

		float totalAmplitude = 0.0f;
		float amplitude = _amplitude;

		int i = _octaves - 1;

		int x, y;

		while (i > -1) {
			float[] octaveNoise = generateOctave (i);
			x = y = 0;

			amplitude *= _step;
			totalAmplitude += amplitude;

			while (x < _width) {
				y = 0;
				while (y < _height) {
					int index = (x * _height) + y;
					_finalNoise [index] += octaveNoise [index] * amplitude;

					y++;
				}
				x++;
			}

			i--;
		}

		x = y = 0;

		_peakHeight = -1.0f;
		_bottomHeight = Mathf.Infinity;

		while (x < _width) {
			y = 0;
			while (y < _height) {
				int index = (x * _width) + y;

				float exp = Mathf.Pow (_finalNoise [index], 2.3f);

				exp = min (exp, 0.85f);

				if (exp <= 0.2f) {
					exp = clamp (exp, 0.1f, 0.2f);
				}

				float val = exp / totalAmplitude;
				_finalNoise [index] = val;

				if (val < _bottomHeight)
					_bottomHeight = val;
				else if (val > _peakHeight)
					_peakHeight = val;

				y++;
			}

			x++;
		}

		_playableHeight = (_bottomHeight + (_peakHeight - _bottomHeight) * _playableHeightPercent);

		if (_createBottom)
			_bottomPlane = (_bottomHeight + (_peakHeight - _bottomHeight) * _bottomPlanePercent);
		else
			_bottomPlane = _bottomHeight;

		float lower = _createBottom ? _bottomPlane : _bottomHeight;

		for (x = 0; x < _width; x++) {
			// Make sure the sides of the terrain volumen are closed to prevent light bleeding
			if (_generatePlayableHeight) {
				if (!_mapBlueprint [x])
					_finalNoise [x] = lower;

				if (!_mapBlueprint [x * _width])
					_finalNoise [x * _width] = lower;

				if (!_mapBlueprint [x * _width + _height - 1])
					_finalNoise [x * _width + _height - 1] = lower;

				if (!_mapBlueprint [x + _height * (_width - 1)])
					_finalNoise [x + _height * (_width - 1)] = lower;
			}

			for (y = 0; y < _height; y++) {
				int index = (x * _width) + y;
				if (_mapBlueprint [index] && _generatePlayableHeight)
					_finalNoise [index] = _playableHeight;
				else if (_createBottom && _finalNoise [index] < _bottomPlane)
					_finalNoise [index] = _bottomPlane;
			}
		}
	}

	private float clamp (float val, float min, float max)
	{
		val = val < min ? min : val > max ? max : val;

		return val;
	}

	private float lerp (float a, float b, float alpha)
	{
		return a + alpha * (b - a);
	}

	private float min (float a, float b)
	{
		return a < b ? a : b;
	}

	public float getNoiseValue (int x, int y)
	{
		int index = (x * _width) + y;

		if (index < 0 || index > _finalNoise.Length)
			return 0.0f;

		return _finalNoise [index];
	}

	public float getPlayableHeight ()
	{
		return _playableHeight;
	}

	public float getBottomPlane ()
	{
		return _bottomPlane;
	}

	public float getPeakHeight ()
	{
		return _peakHeight;
	}

	public float getBottomHeight ()
	{
		return _bottomHeight;
	}

	public int getSeed ()
	{
		return _seed; 
	}
}
