using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management; 
public class PistolGestureDual : MonoBehaviour
{
    [Header("XR Hands")]
    [Tooltip("Si es null, intentará encontrar el XRHandSubsystem en runtime.")]
    public XRHandSubsystem handSubsystem;

    [Header("Gesto: pose de pistola")]
    public float extendedTipDistance = 0.065f; // ~6.5 cm
    public float curledTipDistance = 0.045f; // ~4.5 cm

    [Header("Gesto: flick de muñeca hacia arriba")]
    public float wristFlickAngleDeg = 15f;
    public float flickWindowSeconds = 0.12f;

    [Header("Control disparo")]
    //public float shootCooldown = 0.20f;

    public GameObject player;
    [Header("Eventos de disparo")]
    public UnityEvent OnLeftShoot;
    public UnityEvent OnRightShoot;

    public GameObject bulletPrefab;

    public float spawnForwardOffset = 0.02f;

    private Killable playerKillable;

    struct HandState
    {
        public bool pistolPose;
        public Quaternion lastPalmRot;
        public float lastRotTime;
        public float accumulatedAngle;
        public float lastShootTime;
        public bool initialized;

        public float poseHold;
    }

    private HandState left;
    private HandState right;

    void Awake()
    {
        if (handSubsystem == null)
            handSubsystem = FindXRHandSubsystem();

        if (player == null)
        {
            Debug.LogError("Player reference is not assigned in PistolGestureShoot.");
        }
        playerKillable = player.GetComponent<Killable>();
        if (playerKillable == null)
        {
            Debug.LogError("PistolGestureShoot: No se encontró el componente Killable en el jugador.");
        }
    }

    void Update()
    {
        // Si aún no está listo al arrancar, intentamos encontrarlo hasta que aparezca
        if (handSubsystem == null)
        {
            handSubsystem = FindXRHandSubsystem();
            if (handSubsystem == null) return;
        }

        ProcessHand(handSubsystem.leftHand, true, ref left, OnLeftShoot);
        ProcessHand(handSubsystem.rightHand, false, ref right, OnRightShoot);
    }

    void ProcessHand(XRHand hand, bool isLeft, ref HandState state, UnityEvent shootEvent)
    {
        if (!hand.isTracked)
        {
            state.pistolPose = false;
            state.initialized = false;
            state.accumulatedAngle = 0f;
            return;
        }

        // 1) Pose de pistola
        state.pistolPose = IsPistolPose(hand);

        var playerKillable = player.GetComponent<Killable>();
        if (playerKillable == null)
        {
            Debug.LogError("PistolGestureShoot: No se encontró el componente Killable en el jugador.");
            return;
        }

        bool cooldownOk = (Time.time - state.lastShootTime) >= playerKillable.GetFireRate();
        // 3) Disparo
        if (state.pistolPose && cooldownOk)
        {
            state.lastShootTime = Time.time;
            state.accumulatedAngle = 0f;
            shootEvent?.Invoke();
            Debug.Log(isLeft ? "💥 Left shoot" : "💥 Right shoot");
            FireFromHand(hand);
        }
    }

    bool IsPistolPose(XRHand hand)
    {
        if (!TryGetJointPose(hand, XRHandJointID.Palm, out Pose palm)) return false;
        Vector3 palmPos = palm.position;

        bool indexExtended = IsExtended(hand, XRHandJointID.IndexTip, palmPos, extendedTipDistance);
        bool thumbExtended = IsExtended(hand, XRHandJointID.ThumbTip, palmPos, extendedTipDistance);

        bool middleCurled = IsCurled(hand, XRHandJointID.MiddleTip, palmPos, curledTipDistance);
        bool ringCurled = IsCurled(hand, XRHandJointID.RingTip, palmPos, curledTipDistance);
        bool pinkyCurled = IsCurled(hand, XRHandJointID.LittleTip, palmPos, curledTipDistance);

        return indexExtended && thumbExtended && middleCurled && ringCurled && pinkyCurled;
    }

    bool IsExtended(XRHand hand, XRHandJointID tipId, Vector3 palmPos, float minDistance)
    {
        if (TryGetJointPose(hand, tipId, out Pose tip))
            return Vector3.Distance(tip.position, palmPos) >= minDistance;
        return false;
    }

    bool IsCurled(XRHand hand, XRHandJointID tipId, Vector3 palmPos, float maxDistance)
    {
        if (TryGetJointPose(hand, tipId, out Pose tip))
            return Vector3.Distance(tip.position, palmPos) <= maxDistance;
        return false;
    }

    bool TryGetJointPose(XRHand hand, XRHandJointID id, out Pose pose)
    {
        var joint = hand.GetJoint(id);
        if (joint.TryGetPose(out pose)) return true;
        pose = default;
        return false;
    }

    XRHandSubsystem FindXRHandSubsystem()
    {
        // Forma estable con XR Plugin Management
        var loader = XRGeneralSettings.Instance?.Manager?.activeLoader;
        if (loader != null)
        {
            var hs = loader.GetLoadedSubsystem<XRHandSubsystem>();
            if (hs != null) return hs;
        }
        return null;
    }
    
    void FireFromHand(XRHand hand)
    {
        if (bulletPrefab == null) return;
        if (!TryGetJointPose(hand, XRHandJointID.IndexTip, out Pose tipPose)) return;

          if (!TryGetJointPose(hand, XRHandJointID.IndexIntermediate, out Pose prevPose))
            prevPose = tipPose; // fallback (no debería)

        Vector3 shootDir = (tipPose.position - prevPose.position).normalized;

        Instantiate(bulletPrefab, tipPose.position + shootDir * spawnForwardOffset, tipPose.rotation);
    }
}
