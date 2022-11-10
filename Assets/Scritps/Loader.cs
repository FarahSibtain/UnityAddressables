using Snobal.DesignPatternsUnity_0_0;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Loader : SingletonMonoBehaviour<Loader>
{
    [SerializeField]
    private LeanTweenType easeType = LeanTweenType.easeOutQuad;

    [SerializeField]
    private float alphaTweenFrom = 0.0f;

    [SerializeField]
    private float alphaTweenTo = 1.0f;

    private RawImage loaderContainer;

    private TextMeshProUGUI loaderText;

    public bool IsLoading { get; set; } = false;

    // private List<TweenerCore<Color, Color, ColorOptions>>
}
