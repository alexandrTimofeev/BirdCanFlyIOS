using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIPunchFX : MonoBehaviour
{
    [SerializeField] private UIPunchInstruction[] instructions;

    private readonly Dictionary<string, List<UIPunchInstruction>> dicInstructions = new();
    private readonly Dictionary<Transform, Tween> scaleTweens = new();
    private readonly Dictionary<Transform, Tween> positionTweens = new();
    private readonly Dictionary<Transform, Tween> rotationTweens = new();

    private void Start()
    {
        for (int i = 0; i < instructions.Length; i++)
        {
            var instr = instructions[i];
            if (!dicInstructions.ContainsKey(instr.ID))
                dicInstructions[instr.ID] = new List<UIPunchInstruction>();

            dicInstructions[instr.ID].Add(instr);
        }
    }

    public void InvokeInstructions(string id)
    {
        if (!dicInstructions.TryGetValue(id, out var list)) return;

        foreach (var instr in list)
        {
            foreach (var target in instr.targets)
            {
                if (target == null) continue;

                // Scale
                if (instr.isUseScalePunch)
                {
                    if (scaleTweens.TryGetValue(target, out Tween tween))
                    {
                        tween.Kill();
                        target.localScale = Vector3.one;
                    }

                    Tween newTween = target.DOPunchScale(instr.scaleSize, instr.scaleDuration, instr.scaleVibration)
                        .SetEase(Ease.OutQuad);
                    scaleTweens[target] = newTween;
                }

                // Position
                if (instr.isUsePositionPunch)
                {
                    if (positionTweens.TryGetValue(target, out Tween tween))
                    {
                        tween.Kill();
                        target.localPosition = Vector3.zero;
                    }

                    Tween newTween = target.DOPunchPosition(instr.positionSize, instr.positionDuration, instr.positionVibration)
                        .SetEase(Ease.OutQuad);
                    positionTweens[target] = newTween;
                }

                // Rotation
                if (instr.isUseRotationPunch)
                {
                    if (rotationTweens.TryGetValue(target, out Tween tween))
                    {
                        tween.Kill();
                        target.localRotation = Quaternion.identity;
                    }

                    Tween newTween = target.DOPunchRotation(instr.rotationSize, instr.rotationDuration, instr.rotationVibration)
                        .SetEase(Ease.OutQuad);
                    rotationTweens[target] = newTween;
                }
            }
        }
    }
}

[System.Serializable]
public class UIPunchInstruction
{
    public string ID;
    public Transform[] targets;

    [Header("Scale")]
    public bool isUseScalePunch;
    public Vector3 scaleSize = Vector3.one * 0.2f;
    public float scaleDuration = 0.3f;
    public int scaleVibration = 10;

    [Header("Position")]
    public bool isUsePositionPunch;
    public Vector3 positionSize = Vector3.one * 10f;
    public float positionDuration = 0.3f;
    public int positionVibration = 10;

    [Header("Rotation")]
    public bool isUseRotationPunch;
    public Vector3 rotationSize = Vector3.one * 10f;
    public float rotationDuration = 0.3f;
    public int rotationVibration = 10;
}