using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;





public class ShockWave : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip shockWaveSound;

    public float tipMinDist = 0.06f;

    public float thumbMinDist = 0.05f;

    public float radius = 5f;

    [Header("Clap (coincidencia)")]
    public float palmsMaxDistance = 0.10f;   // 10 cm
    [Range(0f, 1f)] public float facingDot = 0.6f; // palmas “enfrentadas” (normales opuestas)
    public float poseHold = 0.06f;            // tiempo mínimo palma abierta
    public float coincidenceWindow = 0.20f;   // margen entre mano izq/der
    public float cooldown = 0.5f;             // anti-spam
    public GameObject player;

    private Killable killablePlayer;

    public XRHandSubsystem handSubsystem;

    public Vector3 halfExtents = new Vector3(2.5f, 1f, 2.5f);
    // Start is called before the first frame update

    public float forwardOffset = 0.10f;

    public bool showGizmos = true;

    public Color gizmoColor = new Color(0.2f, 0.8f, 1f, 0.25f);

    public float forceMagnitude = 5f;

    public bool leftHandRightPose = false;

    public bool rightHandRightPose = false;

    public float minimumThresholdDistance = 0.05f;

    public GameObject ringVfxPrefab;

    float lastLeft, lastRight;

    public float windowTime = 0.5f;

    private bool isTryingShockwave = false;

    public bool framesWindow = false;
    public int framesToWait = 10;

    struct HandState { public bool open; public float hold; public float lastTrueTime; }
    HandState left, right;
    float lastShockTime;

    void Awake()
    {
        killablePlayer = player.GetComponent<Killable>();
        if (killablePlayer == null)
        {
            Debug.LogError("ShockWave: No se encontró el componente Killable en el jugador.");
        }

        if (handSubsystem == null)
            handSubsystem = FindXRHandSubsystem();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (handSubsystem == null) return;
        UpdateHand(ref left, handSubsystem.leftHand);
        UpdateHand(ref right, handSubsystem.rightHand);

        // // ambas manos con palma abierta estable
        // bool leftReady = left.open && left.hold >= poseHold;
        // bool rightReady = right.open && right.hold >= poseHold;
        // if (!(leftReady && rightReady)) return;

        // ventana de coincidencia
        if (Mathf.Abs(left.lastTrueTime - right.lastTrueTime) > coincidenceWindow) return;

        // palmas cerca y enfrentadas
        if (!PalmsCloseAndFacing(out Vector3 origin)) return;

        if (Time.time - lastShockTime < cooldown) return;
        lastShockTime = Time.time;

        TriggerShockWave(origin);


    }

    void UpdateHand(ref HandState st, XRHand hand)
    {
        if (!hand.isTracked) { st.open = false; st.hold = 0f; return; }

        bool open = IsOpenPalm(hand);
        st.open = open;
        st.hold = open ? st.hold + Time.deltaTime : 0f;
        if (open) st.lastTrueTime = Time.time;
    }

    bool IsOpenPalm(XRHand hand)
    {
        if (!TryPose(hand, XRHandJointID.Palm, out var palm)) return false;
        Vector3 p = palm.position;

        // dedos
        bool i = TipFar(hand, XRHandJointID.IndexTip, p, tipMinDist);
        bool m = TipFar(hand, XRHandJointID.MiddleTip, p, tipMinDist);
        bool r = TipFar(hand, XRHandJointID.RingTip, p, tipMinDist);
        bool l = TipFar(hand, XRHandJointID.LittleTip, p, tipMinDist);
        bool t = TipFar(hand, XRHandJointID.ThumbTip, p, thumbMinDist);

        return i && m && r && l && t;
    }

    bool PalmsCloseAndFacing(out Vector3 origin)
    {
        origin = Vector3.zero;
        var L = handSubsystem.leftHand;
        var R = handSubsystem.rightHand;
        if (!L.isTracked || !R.isTracked) return false;
        if (!TryPose(L, XRHandJointID.Palm, out var LP) ||
            !TryPose(R, XRHandJointID.Palm, out var RP)) return false;

        // distancia
        if (Vector3.Distance(LP.position, RP.position) > palmsMaxDistance) return false;

        // “normales” de palma: suelen ser +Z local (ajusta a +Z/-Z si ves lo contrario)
        Vector3 nL = LP.rotation * Vector3.up;
        Vector3 nR = RP.rotation * Vector3.up;

        float dot = Vector3.Dot(nL, -nR); // 1 = perfectamente enfrentadas

        Debug.Log("ShockWave: Dot de palmas: " + dot);

        if (dot < facingDot) return false;

        origin = (LP.position + RP.position) * 0.5f;
        return true;
    }

    bool TipFar(XRHand hand, XRHandJointID id, Vector3 palmPos, float min)
    {
        return TryPose(hand, id, out var tip) && (tip.position - palmPos).sqrMagnitude >= min * min;
    }

    bool TryPose(XRHand hand, XRHandJointID id, out Pose pose)
    {
        var j = hand.GetJoint(id);
        return j.TryGetPose(out pose);
    }


    bool VerifyHandsDistance(out Vector3 originPalms)
    {
        var leftHand = handSubsystem.leftHand;
        var rightHand = handSubsystem.rightHand;
        originPalms = Vector3.zero;

        if (leftHand.isTracked && rightHand.isTracked)
        {
            var leftPalm = leftHand.GetJoint(XRHandJointID.Palm);
            var rightPalm = rightHand.GetJoint(XRHandJointID.Palm);

            if (leftPalm.TryGetPose(out Pose leftPose) && rightPalm.TryGetPose(out Pose rightPose))
            {
                float distance = Vector3.Distance(leftPose.position, rightPose.position);
                Debug.Log("ShockWave: Distancia entre palmas: " + distance);
                originPalms = (leftPose.position + rightPose.position) * 0.5f;
                return distance <= minimumThresholdDistance;
            }
        }
        return false;

    }

    public void SetLeftHandRightPose(bool state)
    {
        leftHandRightPose = state;

        Debug.Log("Left hand right pose set to " + state);
        if (state)
        {
            lastLeft = Time.time;

        }
        StartCoroutine(TryTriggerShockWave());

    }

    public void SetRightHandRightPose(bool state)
    {
        rightHandRightPose = state;
        Debug.Log("Right hand right pose set to " + state);
        if (state)
        {
            lastRight = Time.time;

        }

        StartCoroutine(TryTriggerShockWave());

    }

    IEnumerator TryTriggerShockWave()
    {
        if (isTryingShockwave) yield break;

        isTryingShockwave = true;

        for (int i = 0; i < framesToWait; i++)
        {
            if (VerifyHandsDistance(out Vector3 originPalms) && Mathf.Abs(lastLeft - lastRight) < windowTime)
            {
                TriggerShockWave(originPalms);
                break;
            }

            yield return null;
        }
        isTryingShockwave = false;

    }



    void TriggerShockWave(Vector3 originPalms)
    {
        Debug.Log("ShockWave: Onda expansiva activada.");
        Vector3 center = originPalms + Camera.main.transform.forward * forwardOffset;
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);

        if (audioSource != null && shockWaveSound != null)
        {
            audioSource.PlayOneShot(shockWaveSound);
        }

        if (ringVfxPrefab != null)
        {
            var childPS = ringVfxPrefab.GetComponentInChildren<ParticleSystem>(true);
            if (childPS != null)
            {
                var main = childPS.main;
                main.startSize3D = true;
                main.startSizeX = radius * 2f;
                main.startSizeY = radius * 2f;
                main.startSizeZ = radius * 2f;
            }
        }
    
        foreach (var hitCollider in hitColliders)
        {
            var killableEnemy = hitCollider.gameObject.GetComponent<Killable>();


            if (killableEnemy != null && killableEnemy.GetTeam() != killablePlayer.GetTeam())
            {
                killableEnemy.TakeDamage(killablePlayer.GetDamage());
                Debug.Log("ShockWave: Enemigo dañado por la onda expansiva.");

                var enemyFollowerComponent = hitCollider.gameObject.GetComponent<EnemyFollow>();
                if (enemyFollowerComponent != null)
                {
                    Vector3 forceDirection = (hitCollider.transform.position - originPalms).normalized;
                    enemyFollowerComponent.ApplyKnockback(forceDirection, forceMagnitude, 1.0f);
                    Debug.Log("ShockWave: Enemigo empujado por la onda expansiva.");
                }
            }


        }



    }


    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = gizmoColor;
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;
        Vector3 up = transform.up;
        Vector3 center = origin + forward * forwardOffset;
        Quaternion rot = Quaternion.LookRotation(forward, up);

        Matrix4x4 m = Matrix4x4.TRS(center, rot, Vector3.one);
        Gizmos.matrix = m;
        Gizmos.DrawCube(Vector3.zero, halfExtents * 2f);
        Gizmos.matrix = Matrix4x4.identity;
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



}
