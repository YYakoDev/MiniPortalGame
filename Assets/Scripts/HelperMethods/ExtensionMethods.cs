using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public static class ExtensionMethods
{

    public static void SetPosition(this Camera camera, Vector3 position)
    {
        position.z = camera.transform.position.z;
        camera.transform.position = position;
    }


    /*//ANIMATOR STUFF
    public static void PlayWithEndTime(this Animator animator,string animationName)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f) animator.Play(animationName);
    }
    public static void ForcePlay(this Animator animator, int animationHash)
    {
        animator.StopPlayback();
        animator.Play(animationHash);
    }*/

    /// <summary>
    /// Check Component will try to get the component if it is not null. Returning true if sucessfully get, and false if the component is not attached or some reference is null  
    /// </summary>
    /// <param name="gameObj"></param>
    /// <param name="component"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool CheckComponent<T>(this GameObject gameObj, ref T component) where T: Component
    {
        if (gameObj == null) return false;
        return component != null || gameObj.TryGetComponent(out component);
    }

    public static void CheckOrAddComponent<T>(this GameObject gameObj, ref T componentToCheck) where T : Component
    {
        if (!gameObj.CheckComponent(ref componentToCheck))
        {
            componentToCheck = gameObj?.AddComponent<T>();
        }
    }

    //AUDIO STUFF
    public static void PlayWithVaryingPitch(this AudioSource audio, AudioClip clip, float startingPitch = 1f)
    {
        float pitchOffset = Random.Range(-0.02f, 0.035f);
        audio.pitch = startingPitch + pitchOffset;
        audio.PlayOneShot(clip);
    }

    public static void PlayWithCooldown(this AudioSource audio, AudioClip clip, float cooldown, ref float nextPlayTime)
    {
        if(Time.time < nextPlayTime) return;
        nextPlayTime = Time.time + cooldown;
        audio.PlayWithVaryingPitch(clip);
    }


    // BUTTONS
    public static void AddEventListener(this Button button, Action OnClick)
    {
        button.onClick.AddListener(() => {OnClick(); });
    }
    public static void AddEventListener<T>(this Button button, Action<T> OnClick, T argument)
    {
        button.onClick.AddListener(() => {OnClick(argument); });
    }
        public static void AddEventListener<T, T2>(this Button button, Action<T,T2> OnClick, T argument, T2 argument2)
    {
        button.onClick.AddListener(() => {OnClick(argument, argument2); });
    }
    public static void RemoveAllEvents(this Button button)
    {
        button.onClick.RemoveAllListeners();
    }



    //UI POSITION TRANSLATION 
    public static Vector2 TranslateUiToWorldPoint(this Canvas canvas, Vector2 position)
    {
        float scaleFactor = canvas.scaleFactor;
        var canvasRect = canvas.GetComponent<RectTransform>();
        float canvasCenterX = canvasRect.position.x; //this two positions are already expressed as world points and also its the center of the canvas
        float canvasCenterY = canvasRect.position.y;

        //Debug.Log(canvasCenterX);
        //float percentX = position.x / canvasWidth;
        //float percentY = position.y / canvasHeight;

        Vector2 worldPoint = new Vector2
        (
            canvasCenterX + position.x * scaleFactor,
            canvasCenterY + position.y * scaleFactor            
        );
        return worldPoint;
    }

    public static int GetRandomIndexExcept(this Array array, params int[] exceptions) => HelperMethods.GetRandomIndexExcept(array.Length);
    
    public static int GetRandomIndexExcept(this IList list, params int[] exceptions)
    {
        //DEBUG ONLY
        for (int i = 0; i < exceptions.Length; i++)
        {
            var except = exceptions[i];
            Debug.Log("Exception: == " + except);
        }
        return HelperMethods.GetRandomIndexExcept(list.Count);
    }


    public static bool Contains<T>(this T[] array, T itemToSearch)
    {
        return ContainsItem(array, itemToSearch) != -1;
    }
    
    /// <summary>
    /// Searchs on the array if the given item is present, returns the index if it is, if not returns -1.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="itemToSearch"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int ContainsItem<T>(this T[] array, T itemToSearch)
    {
        if (array == null || array.Length == 0) return -1;
        for(int i = 0; i < array.Length; i++)
        {
            var t = array[i];
            if(t == null) continue;
            if (t.Equals(itemToSearch))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Adds the given not null element to the array and returns the index at which it was added
    /// </summary>
    /// <param name="arrayClass"></param>
    /// <param name="array"></param>
    /// <param name="newItem"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int Add<T>(this Array arrayClass, ref T[] array, T newItem)
    {
        if (array == null)
        {
            Debug.LogWarning("The given array is null");
            return -1;
        }
        if (newItem == null) return -1;
        var length = array.Length;
        Array.Resize<T>(ref array, length + 1);
        array[length] = newItem;
        return length;
    }


    public static float Round(this float value, MidpointRounding midpointRounding = MidpointRounding.ToEven)
    {
        return (float)Math.Round(value, midpointRounding);
    }
    
    public static float RoundAwayFromZero(this float value) => Round(value, MidpointRounding.AwayFromZero);
    
    public static Vector2 Round(this ref Vector2 v, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
    {
        v.x = v.x.Round(midpointRounding);
        v.y = v.y.Round(midpointRounding);
        return v;
    }
    public static Vector3 Round(this ref Vector3 v, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
    {
        v.x = v.x.Round(midpointRounding);
        v.y = v.y.Round(midpointRounding);
        v.z = v.z.Round(midpointRounding);
        return v;
    }

    public static Vector3 Floor(this ref Vector3 v)
    {
        v.x = Mathf.Floor(v.x);
        v.y = Mathf.Floor(v.y);
        v.z = Mathf.Floor(v.z);
        return v;
    }

    public static bool GreaterThan(this Vector2 v, Vector2 v2)
    {
        return (v.x > v2.x && v.y > v2.y);
    }

    public static bool LessThan(this Vector2 v, Vector2 v2) => !GreaterThan(v, v2);

    public static bool GreaterThan(this Vector3 v, Vector3 v2)
    {
        return (v.x > v2.x && v.y > v2.y);
    }

    public static bool LessThan(this Vector3 v, Vector3 v2) => !GreaterThan(v, v2);

    public static bool Approximately(this Vector2 v, Vector2 v2)
    {
        return (Mathf.Approximately(v.x, v2.x) && Mathf.Approximately(v.y, v2.y));
    }

    public static void Log(this string str)
    {
        Debug.Log(str);
    }

    public static void Resize<T>(this Array arrayClass, ref T[] array, int newSize)
    {
        Array.Resize(ref array, newSize);
    }
    
    public static Vector2 Abs(this ref Vector2 v)
    {
        v.x = Mathf.Abs(v.x);
        v.y = Mathf.Abs(v.y);
        return v;
    }
    
    public static Vector3 Abs(this ref Vector3 v)
    {
        v.x = Mathf.Abs(v.x);
        v.y = Mathf.Abs(v.y);
        v.z = Mathf.Abs(v.z);
        return v;
    }

    public static Vector2 MultiplyBy(this ref Vector2 v, Vector2 m)
    {
        v.x *= m.x;
        v.y *= m.y;
        return v;
    }
    
    public static Vector3 MultiplyBy(this ref Vector3 v, Vector3 m)
    {
        v.x *= m.x;
        v.y *= m.y;
        v.z *= m.z;
        return v;
    }
    
    public static int RoundToInt(this ref float f)
    {
        return Mathf.RoundToInt(f);
    }
    
    public static Vector2Int Abs(this ref Vector2Int v)
    {
        v.x = Mathf.Abs(v.x);
        v.y = Mathf.Abs(v.y);
        return v;
    }

    public static float GetBiggerPoint(this Vector2 v) => v.x > v.y ? v.x : v.y;
    public static float GetBiggerPoint(this Vector3 v) => v.x > v.y ? v.x : v.y;
    public static float GetSmallerPoint(this Vector2 v) => v.x < v.y ? v.x : v.y;
    public static float GetSmallerPoint(this Vector3 v) => v.x < v.y ? v.x : v.y;
    
    public static float Abs(this float f)
    {
        return Mathf.Abs(f);
    }
    
    public static void AddOnNull<T>(this T[] array, T newItem)
    {
        if (array == null)
        {
            Debug.LogWarning("The given array is null");
            return;
        }
        if (newItem == null) return;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null) continue;
            array[i] = newItem;
            return;
        }
        array.Add(ref array, newItem);
    }

    public static T GetRandom<T>(this T[] array)
    {
        if (array == null || array.Length == 0)
        {
            Debug.LogWarning("The given array is null or empty");
            return default;
        }
        int randomIndex = Random.Range(0, array.Length);
        return array[randomIndex];
    }
    
    private static Material _whiteBlinkMat, _redBlinkMat;
    public static Material WhiteBlinkMat
    {
        get
        {
            if (_whiteBlinkMat != null) return _whiteBlinkMat;
            _whiteBlinkMat = Resources.Load<Material>("Materials/WhiteBlinkMaterial");
            return _whiteBlinkMat;
        }
    }
    public static Material RedBlinkMat
    {
        get
        {
            if (_redBlinkMat != null) return _redBlinkMat;
            _redBlinkMat = Resources.Load<Material>("Materials/RedBlinkMaterial");
            return _redBlinkMat;
        }
    }
    
    public static void Blink(this SpriteRenderer renderer, float duration = 0.1f, BlinkType blinkType = BlinkType.White)
    {
        if (renderer == null) return;
        
       
        var originalMaterial = IsBlinkMat(renderer.material) ? renderer.material : new Material(Shader.Find("Standard"));
        Blink(renderer, originalMaterial, duration, blinkType);
        return;

        bool IsBlinkMat(Material mat)
        {
            var result = blinkType switch
            {
                BlinkType.White => mat == WhiteBlinkMat,
                BlinkType.Red => mat == RedBlinkMat,
                _ => false
            };
            return result;
        }
    }

    public static void Blink(this SpriteRenderer renderer, Material originalMat, float duration = 0.1f, BlinkType blinkType = BlinkType.White)
    {
        var blinkMat = blinkType switch
        {
            BlinkType.White => WhiteBlinkMat,
            BlinkType.Red => RedBlinkMat,
            _ => WhiteBlinkMat
        };
        if (blinkMat == null)
        {
            Debug.LogError("Blink material not found. Make sure the material is in the Resources folder.");
            return;
        }
        renderer.material = blinkMat;
        YYExtensions.Instance.ExecuteMethodAfterTime(duration, () =>
        {
            if (renderer == null) return;
            renderer.material = originalMat;
        });
    }
}

public enum BlinkType
{
    White,
    Red
}
