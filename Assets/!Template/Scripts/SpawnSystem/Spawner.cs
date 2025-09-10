using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Vector2 area;
    [SerializeField] private float destroyWait = 12f;
    private float speed = 1f;

    private List<Coroutine> _spawnCoroutines = new List<Coroutine>();
    public Action<string, float> OnInstructionProgress;

    public void SetPhase(SpawnPhase phase)
    {
        foreach (var c in _spawnCoroutines)
            StopCoroutine(c);
        _spawnCoroutines.Clear();

        foreach (var instr in phase.Instructions)
        {
            Coroutine c = StartCoroutine(SpawnRoutine(instr));
            _spawnCoroutines.Add(c);
        }
    }

    private IEnumerator SpawnRoutine(SpawnInstruction instr)
    {
        int specalNextSpawn = instr.spawnSpecialCount;
        SpawnableItem specalSpawn = instr.spawnSpecial;
        while (true)
        {
            float wait = Random.Range(instr.SpawnInterval.x, instr.SpawnInterval.y);
            yield return new WaitForSeconds(wait / speed);
            while (GamePause.IsPause)
                yield return new WaitForEndOfFrame();

            var item = instr.GetRandomItem();
            if (specalNextSpawn == 0 && specalSpawn.Prefab != null)
            {
                item = specalSpawn;
                specalNextSpawn = instr.spawnSpecialCount;
            }
            if (item?.Prefab == null) continue;

            GameObject prefab = item.Prefab;
            GameObject ngo = Spawn(prefab);

            if (ngo != null)
            {
                switch (instr.spawnAnimation)
                {
                    case SpawnAnimationType.None:
                        break;
                    case SpawnAnimationType.Scale:
                        ngo.transform.localScale = Vector3.zero;
                        ngo.transform.DOScale(1f, instr.durarion);
                        break;
                    default:
                        break;
                }

                OnInstructionProgress?.Invoke(instr.id, (float)(instr.spawnSpecialCount - specalNextSpawn) / (float)instr.spawnSpecialCount);
            }
            specalNextSpawn--;
        }
    }
    public GameObject Spawn(GameObject prefab)
    {
        float x = Random.Range(-area.x / 2f, area.x / 2f);
        float y = Random.Range(-area.y / 2f, area.y / 2f);
        GameObject ngo = Instantiate(prefab, transform.position + new Vector3(x, y, 0f), Quaternion.identity);
        Destroy(ngo, destroyWait);
        return ngo;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, area);
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}