using System;
using UnityEngine;

public class MoveParticlesToTarget : MonoBehaviour
{
    [SerializeField] private Transform _target; // The target (e.g., a progress bar)
    [SerializeField] private float _speed = 1.0f; // Speed of particle movement
    [SerializeField] private float _smoothFactor = 0.1f; // Smoothness of particle movement
    [SerializeField] private float _eps = 0.5f; // Distance treshold

    public event Action OnParticlesMovementStart;
    public event Action OnFirstParticleReachedTarget;
    public event Action OnParticlesMovementFinish;

    private ParticleSystem _particleSystem;
    private ParticleSystem.Particle[] _particles;
    private Vector2 _targetPosition;
    private bool _isMovementStopped = true;
    private bool _allParticlesReachedTarget = false;
    private bool _didFirstParticleReachTarget = false;

    void Start()
    {
        // Initialize vars
        _particleSystem = GetComponent<ParticleSystem>();
        _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
        _targetPosition = _target.position;

        // Set the simulation space to World so particles move in world coordinates
        var main = _particleSystem.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
    }

    void Update()
    {
        if (_isMovementStopped) return;

        // Get the number of active particles
        int numParticlesAlive = _particleSystem.GetParticles(_particles);
        if (numParticlesAlive == 0) return;

        // Convert the target's position to world coordinates
        if (_target.transform is RectTransform)
        {
            _targetPosition = GetWorldPositionOfRectTransform((RectTransform)_target);
        }

        // Move each particle towards the target position smoothly
        _allParticlesReachedTarget = true;
        for (int i = 0; i < numParticlesAlive; i++)
        {
            _particles[i].position = Vector2.Lerp(
                (Vector2)_particles[i].position,
                _targetPosition,
                _smoothFactor * Time.deltaTime * _speed
            );
            if (Vector2.Distance((Vector2)_particles[i].position, _targetPosition) > _eps)
            {
                _allParticlesReachedTarget = false;
            } else if (!_didFirstParticleReachTarget)
            {
                OnFirstParticleReachedTarget?.Invoke();
                _didFirstParticleReachTarget = true;
            }
        }

        // Apply the modified particle data back to the ParticleSystem
        _particleSystem.SetParticles(_particles, numParticlesAlive);

        if (_allParticlesReachedTarget) 
        {
            OnParticlesMovementFinish?.Invoke();
            _isMovementStopped = true;
        }
    }

    /// <summary>
    /// Starts moving particles to the target.
    /// </summary>
    public void StartMovement()
    {
        _isMovementStopped = false;
        OnParticlesMovementStart?.Invoke();
    }

    /// <summary>
    /// Converts the position of a RectTransform to world coordinates.
    /// </summary>
    /// <param name="rectTransform">The RectTransform to convert.</param>
    /// <returns>The world position of the RectTransform.</returns>
    private Vector2 GetWorldPositionOfRectTransform(RectTransform rectTransform)
    {
        // Get the Canvas that the RectTransform belongs to
        Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("RectTransform must be a child of a Canvas.");
            return Vector2.zero;
        }

        // Convert the RectTransform's position to a screen point
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);

        // Convert the screen point to world coordinates
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            screenPoint,
            canvas.worldCamera,
            out Vector3 worldPosition
        );

        return worldPosition;
    }
}
