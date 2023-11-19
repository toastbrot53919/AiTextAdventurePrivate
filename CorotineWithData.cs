using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
public class CoroutineWithData
{
    private MonoBehaviour owner;
    private TaskCompletionSource<object> tcs;

    public CoroutineWithData(MonoBehaviour owner)
    {
        this.owner = owner;
        tcs = new TaskCompletionSource<object>();
    }

    public Task<object> Run(IEnumerator coroutine)
    {
        owner.StartCoroutine(RunCoroutine(coroutine));
        return tcs.Task;
    }

    private IEnumerator RunCoroutine(IEnumerator coroutine)
    {
        while (coroutine.MoveNext())
        {
            yield return coroutine.Current;
        }
        tcs.SetResult(null); // This is where you would set the result of the coroutine
    }
}

