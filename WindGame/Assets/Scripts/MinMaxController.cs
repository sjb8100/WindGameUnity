﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MinMaxController : MonoBehaviour {
    public Slider maxSlider;
    public Slider minSlider;

    public Text nameText;
    public Text minText;
    public Text maxText;
    public InputField minInputField;
    public InputField maxInputField;

    public Image minTargetGraphic;
    public Image maxTargetGraphic;

    public Text minUnitText;
    public Text maxUnitText;

    [HideInInspector]
    public MinMaxFloatProperty minMaxFloatProperty;

    // Use this for initialization
    void Start()
    {
        nameText.text = minMaxFloatProperty.propertyName;
        minUnitText.text = minMaxFloatProperty.unit;
        maxUnitText.text = minMaxFloatProperty.unit;
        minText.text = minMaxFloatProperty.minPropertyName;
        maxText.text = minMaxFloatProperty.maxPropertyName;
        minSlider.minValue = minMaxFloatProperty.minValue;
        minSlider.maxValue = minMaxFloatProperty.maxValue;
        maxSlider.minValue = minMaxFloatProperty.minValue;
        maxSlider.maxValue = minMaxFloatProperty.maxValue;
        minSlider.value = minMaxFloatProperty.minLastSetting;
        maxSlider.value = minMaxFloatProperty.maxLastSetting;

        minInputField.contentType = InputField.ContentType.DecimalNumber;
        maxInputField.contentType = InputField.ContentType.DecimalNumber;

        minInputField.text = minSlider.value.ToString();
        maxInputField.text = maxSlider.value.ToString();

        minInputField.onValueChanged.AddListener(delegate { MinInputFieldChange(minInputField.text); });
        maxInputField.onValueChanged.AddListener(delegate { MaxInputFieldChange(maxInputField.text); });

        minSlider.onValueChanged.AddListener(delegate { MinSliderChange(minSlider.value); });
        maxSlider.onValueChanged.AddListener(delegate { MaxSliderChange(maxSlider.value); });

        MinInputFieldChange(minInputField.text);
        MaxInputFieldChange(maxInputField.text);

    }

    void MinInputFieldChange(string value)
    {
        if (value != string.Empty)
        {

            if (float.Parse(value) > maxSlider.value)
                minSlider.value = maxSlider.value;
            else
                minSlider.value = float.Parse(value);

            if (minMaxFloatProperty.minGraphicsFunction != null)
                minMaxFloatProperty.minGraphicsFunction.Invoke(minMaxFloatProperty.callObject, new object[] { minSlider.value });

            minMaxFloatProperty.minProperty = minSlider.value;
            minMaxFloatProperty.minLastSetting = minSlider.value;
        }
    }

    void MaxInputFieldChange(string value)
    {
        if (value != string.Empty)
        {

            if (minSlider.value > float.Parse(value))
                maxSlider.value = minSlider.value;
            else
                maxSlider.value = float.Parse(value);
            if (minMaxFloatProperty.maxGraphicsFunction != null)
                minMaxFloatProperty.maxGraphicsFunction.Invoke(minMaxFloatProperty.callObject, new object[] { maxSlider.value });

            minMaxFloatProperty.maxProperty = float.Parse(value);
            minMaxFloatProperty.maxLastSetting = float.Parse(value);
        }
    }

    void MinSliderChange(float value)
    {
        Vector3 pos = minSlider.targetGraphic.transform.position;
        if (minSlider.value > maxSlider.value)
        {
            minSlider.value = maxSlider.value;
            minInputField.text = maxSlider.value.ToString();
        }
        else
            minInputField.text = value.ToString();

        if (minMaxFloatProperty.minGraphicsFunction != null)
            minMaxFloatProperty.minGraphicsFunction.Invoke(minMaxFloatProperty.callObject, new object[] { minSlider.value });

        minMaxFloatProperty.minProperty = minSlider.value;
        minMaxFloatProperty.minLastSetting = minSlider.value;
    }

    void MaxSliderChange(float value)
    {
        if (minSlider.value > maxSlider.value)
        {
            maxSlider.value = minSlider.value;
            maxInputField.text = minSlider.value.ToString();
        }
        else
            maxInputField.text = value.ToString();

        if (minMaxFloatProperty.maxGraphicsFunction != null)
            minMaxFloatProperty.maxGraphicsFunction.Invoke(minMaxFloatProperty.callObject, new object[] { maxSlider.value });

        minMaxFloatProperty.maxProperty = maxSlider.value;
        minMaxFloatProperty.minLastSetting = maxSlider.value;

    }
}
