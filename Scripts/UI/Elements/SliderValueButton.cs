using Godot;
using System;

public partial class SliderValueButton : Button
{
    [Export] Slider slider;
    [Export] ValueMode mode;
    [Export] double value;

    enum ValueMode
    {
        Min,
        Max,
        Value
    }

    public override void _Ready()
    {
        Pressed += OnPressed;
    }

    void OnPressed()
    {
        if (mode == ValueMode.Min) slider.Value = slider.MinValue;
        else if (mode == ValueMode.Max) slider.Value = slider.MaxValue;
        else slider.Value = value;
    }
}