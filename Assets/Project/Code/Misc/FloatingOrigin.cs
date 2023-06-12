// Based on the Unity Wiki FloatingOrigin script by Peter Stirling
// URL: http://wiki.unity3d.com/index.php/Floating_Origin

using UnityEngine;
using UnityEngine.SceneManagement;

public class FloatingOrigin : MonoBehaviour
{
    [Tooltip("Point of reference from which to check the distance to origin.")]
    public Transform referenceObject = null;

    [Tooltip("Distance from the origin the reference object must be in order to trigger an origin shift.")]
    public float Threshold = 5000f;

    [Header("Options")]
    [Tooltip("When true, origin shifts are considered only from the horizontal distance to orign.")]
    public bool Use2DDistance = false;

    [Tooltip("When true, updates ALL open scenes. When false, updates only the active scene.")]
    public bool UpdateAllScenes = true;

    [Tooltip("Should ParticleSystems be moved with an origin shift.")]
    public bool UpdateParticles = true;

    [Tooltip("Should TrailRenderers be moved with an origin shift.")]
    public bool UpdateTrailRenderers = true;

    [Tooltip("Should LineRenderers be moved with an origin shift.")]
    public bool UpdateLineRenderers = true;

    private ParticleSystem.Particle[] parts = null;

    private Transform worldCenterTransform;

    void Start()
    {
        if (!worldCenterTransform)
        {
            // Some calculations use the Y height of objects. Because the floating origin shifts the position of all root objects, we need to keep a reference at pos (0,0,0)
            // This way the calculalations 
        }
    }

    void FixedUpdate()
    {
        if (!referenceObject)
        {
            //If we dont have a refernce object try to get it from the camera controller
            referenceObject = GetComponent<CameraController>().GetCameraRigTransform();
            return;
        }

        Vector3 referencePosition = referenceObject.position;

        if (Use2DDistance)
            referencePosition.y = 0f;

        if (referencePosition.magnitude > Threshold)
        {
            MoveRootTransforms(referencePosition);

            if (UpdateParticles)
                MoveParticles(referencePosition);

            if (UpdateTrailRenderers)
                MoveTrailRenderers(referencePosition);

            if (UpdateLineRenderers)
                MoveLineRenderers(referencePosition);
        }
    }
    private void MoveRootTransforms(Vector3 offset)
    {
        // This is really bad, need to make a better system for floating origin.
        // The reason we have to disable engines and wings is because a sudden shift of all root objects will cause a spike in the lift/drag calculations of airfoils
        foreach(SimpleWing w in FindObjectsOfType<SimpleWing>())
        {
            w.enabled = false;
        }
        //foreach(Engine e in FindObjectsOfType<Engine>())
        //{
        //    e.enabled = false;
        //}
        
        if (UpdateAllScenes)
        {
            for (int z = 0; z < SceneManager.sceneCount; z++)
            {
                foreach (GameObject g in SceneManager.GetSceneAt(z).GetRootGameObjects())
                    g.transform.position -= offset;
            }
        }
        else
        {
            foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
                g.transform.position -= offset;
        }
        foreach (SimpleWing w in FindObjectsOfType<SimpleWing>())
        {
            w.enabled = true;
        }
        //foreach (Engine e in FindObjectsOfType<Engine>())
        //{
        //    e.enabled = true;
        //}
    }
    private void MoveTrailRenderers(Vector3 offset)
    {
        var trails = FindObjectsOfType<TrailRenderer>() as TrailRenderer[];
        foreach (var trail in trails)
        {
            Vector3[] positions = new Vector3[trail.positionCount];

            int positionCount = trail.GetPositions(positions);
            for (int i = 0; i < positionCount; ++i)
                positions[i] -= offset;

            trail.SetPositions(positions);
        }
    }
    private void MoveLineRenderers(Vector3 offset)
    {
        var lines = FindObjectsOfType<LineRenderer>() as LineRenderer[];
        foreach (var line in lines)
        {
            Vector3[] positions = new Vector3[line.positionCount];

            int positionCount = line.GetPositions(positions);
            for (int i = 0; i < positionCount; ++i)
                positions[i] -= offset;

            line.SetPositions(positions);
        }
    }
    private void MoveParticles(Vector3 offset)
    {
        var particles = FindObjectsOfType<ParticleSystem>() as ParticleSystem[];
        foreach (ParticleSystem system in particles)
        {
            if (system.main.simulationSpace != ParticleSystemSimulationSpace.World)
                continue;

            int particlesNeeded = system.main.maxParticles;

            if (particlesNeeded <= 0)
                continue;

            bool wasPaused = system.isPaused;
            bool wasPlaying = system.isPlaying;

            if (!wasPaused)
                system.Pause();

            // ensure a sufficiently large array in which to store the particles
            if (parts == null || parts.Length < particlesNeeded)
            {
                parts = new ParticleSystem.Particle[particlesNeeded];
            }

            // now get the particles
            int num = system.GetParticles(parts);

            for (int i = 0; i < num; i++)
            {
                parts[i].position -= offset;
            }

            system.SetParticles(parts, num);

            if (wasPlaying)
                system.Play();
        }
    }
}