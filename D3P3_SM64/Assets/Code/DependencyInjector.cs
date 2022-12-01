using System.Collections.Generic;
using UnityEngine;

public class DependencyInjector
{
    static Dictionary<System.Type, System.Object> dependencies = new Dictionary<System.Type, System.Object>();
    public static T GetDependency<T>()
    {
        if (!dependencies.ContainsKey(typeof(T)))
        {
            Debug.LogError("Cannot find: " + typeof(T).ToString() +".");
            return default(T);
        }
        return (T)dependencies[typeof(T)];
    }
    public static void AddDependency<T>(System.Object obj)
    {
        if (dependencies.ContainsKey(typeof(T)))
        {
            Debug.Log("There's already an object of type: " + typeof(T).ToString());
            Debug.Log("Object 1: " + dependencies[typeof(T)].GetType().ToString());
            Debug.Log("Object 2: " + obj.GetType().ToString());
            dependencies.Remove(typeof(T));
        }
        dependencies.Add(typeof(T), obj);
    }
}

public class CoinManager : MonoBehaviour, IScoreManager
{
    [SerializeField] float points;
    public event ScoreChanged scoreChangedDelegate;
    void Awake()
    {
        DependencyInjector.AddDependency<IScoreManager>(this);
    }
    public void addPoints(float points)
    {
        this.points += points;
        scoreChangedDelegate?.Invoke(this);
    }
    public float getPoints() { return points; }
}

public interface IScoreManager
{
    void addPoints(float f);
    float getPoints();
    event ScoreChanged scoreChangedDelegate;
}
public delegate void ScoreChanged(IScoreManager scoreManager);
