using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public static class HelperMethods
{
    private static Camera _camera;
    public static Camera MainCamera
    {
        get{
            if(_camera == null) _camera = Camera.main;
            return _camera;
        }
    }


    private static TweenAnimatorMultiple _scaledTimeTweenAnimator, _unscaledTimeTweenAnimator;

    public static TweenAnimatorMultiple ScaledTimeTweenAnimator
    {
        get
        {
            if (_scaledTimeTweenAnimator != null) return _scaledTimeTweenAnimator;
            GameObject animatorObj = new("Static TweenAnimator", typeof(StaticAnimatorDestroyer), typeof(DontDestroyOnLoad));
            _scaledTimeTweenAnimator = animatorObj.AddComponent<TweenAnimatorMultiple>();
            return _scaledTimeTweenAnimator;
        }
    }
    
    public static TweenAnimatorMultiple UnscaledTimeTweenAnimator
    {
        get
        {
            if (_unscaledTimeTweenAnimator != null) return _unscaledTimeTweenAnimator;
            
            GameObject animatorObj = new("Static TweenAnimator", typeof(StaticAnimatorDestroyer), typeof(DontDestroyOnLoad));
            _unscaledTimeTweenAnimator = animatorObj.AddComponent<TweenAnimatorMultiple>();
            _unscaledTimeTweenAnimator.ChangeTimeScalingUsage(TweenAnimator.TimeUsage.UnscaledTime);
            return _unscaledTimeTweenAnimator;
        }
    }
    public static void DestroyAnimators()
    {
        _unscaledTimeTweenAnimator = null;
        _scaledTimeTweenAnimator = null;
    }
    private static AudioSource _audioSource;
    private static AudioSource GetAudioSource()
    {
        if (_audioSource != null) return _audioSource;
        GameObject audioObj = new("Static Audio Source", typeof(StaticAnimatorDestroyer), typeof(DontDestroyOnLoad));
        _audioSource = audioObj.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 0f;
        return _audioSource;
    }
    
    public static void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        var audioSource = GetAudioSource();
        audioSource.PlayOneShot(clip);
    }



    /*private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new();
    public static WaitForSeconds GetWait(float time)
    {
        if(WaitDictionary.TryGetValue(time, out var waitForSeconds)) return waitForSeconds;

        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }*/

    
    private static PointerEventData _eventDataCurrentPosition;
    private static List<RaycastResult> _results;

    public static bool IsOverUI()
    {
        EventSystem currentEventSys = EventSystem.current;
        _eventDataCurrentPosition = new PointerEventData(currentEventSys) {position = (Mouse.current.position.value)};
        _results = new();
        currentEventSys.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }

    public static string AddSpacesToSentence(string text, bool preserveAcronyms)
    {
        if (string.IsNullOrWhiteSpace(text))
           return string.Empty;
        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
                if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                    (preserveAcronyms && char.IsUpper(text[i - 1]) && 
                     i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString();
    }

    public static int RandomNumberExcept(int min, int max, int except)
    {
        int number = Random.Range(min, max);
        while (number == except)
        {
            number = Random.Range(min, max);
        }
        return number;
    }
    public static int GetRandomIndexExcept(int maxPossibilitiesSize, params int[] exceptions)
    {
        var size = maxPossibilitiesSize - exceptions.Length;
        int[] totalNumbers = new int[size];
        
        for (int i = 0; i < totalNumbers.Length; i++)
        {
            if(IndexMatchesExceptions(i)) continue;
            totalNumbers[i] = i;
        }

        bool IndexMatchesExceptions(int index)
        {
            for (int i = 0; i < exceptions.Length; i++)
            {
                var except = exceptions[i];
                if(except == index) return true;
            }
            return false;
        }

        return totalNumbers[Random.Range(0, size)];
    }

    public static int RandomRangeExcept(int minInclusive, int maxExclusive, params int[] exceptions)
    {
        //Debug.Log($"Min size of random range: {min} \n Max size of random range: {max}");
        var size = (maxExclusive - minInclusive) - exceptions.Length;
        if(size <= 0)
        {
            Debug.LogError("could not get a random range, too many exceptions \n exceptions count:  " + exceptions.Length + "   \n size of the array:  " + size);
            return -1;
        }
        int[] totalNumbers = new int[size];
        int iterator = 0;
        for (int i = minInclusive; i < maxExclusive; i++)
        {
            if(IndexMatchesExceptions(i)) continue;
            if(iterator >= size)
            {
                // if this part is executed is probably because one of the exceptions is not actually on the range.
                Debug.Log("Current random iteration:  " + iterator + "   total numbers size:  " + size);
                break;
            }
            totalNumbers[iterator] = i;
            iterator++;
        }
        bool IndexMatchesExceptions(int index)
        {
            for (int i = 0; i < exceptions.Length; i++)
            {
                var except = exceptions[i];
                if(except == index) return true;
            }
            return false;
        }
        return totalNumbers[Random.Range(0, size)];

    }
    
    static double NthRoot(double A, float N)
    {
        return Math.Pow(A, 1.0 / N);
    }

    public static int GetLayerMaskIndex(LayerMask layer)
    {
        var layerValue = layer.value;
        if(layerValue == 0) return 0;
        var exponent = Mathf.Log10(layerValue) / Mathf.Log10(2f);
        int result = Mathf.RoundToInt(exponent);
        return result;
    }
    
    public static float GetBiggerValueFromVector(Vector2 v)
    {
        bool xIsBigger = true;
        xIsBigger = v.x >= v.y;
        return xIsBigger ? v.x : v.y;
    }
    
    public static float GetDistance(int x, int y, Vector2 centerVector)
    {
        float num1 = x - centerVector.x;
        float num2 = y - centerVector.y;
        return Mathf.Sqrt(num1 * num1 + num2 * num2);
    }
}
