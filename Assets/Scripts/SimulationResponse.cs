using UnityEngine;

[System.Serializable]
public class SimulationResponse
{
    public float simulationTime;
    public int collisionCount;
    public SimulationResponse(float _simulationTime, int _collisionCount)
	{
        simulationTime = _simulationTime;
        collisionCount = _collisionCount;
	}
}