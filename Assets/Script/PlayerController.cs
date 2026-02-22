using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class GridMovement2D : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private float cellStepX = 1f;
    [SerializeField] private float cellStepY = 1f;
    [SerializeField] private float moveSpeed = 6f;

    [Header("Collision")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float skin = 0.02f;

    private BoxCollider2D col;
    private Animator anim;
    private bool isMoving;
    private float fixedZ;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        fixedZ = transform.position.z;

        anim.SetBool("move?", false);
    }

    private void Update()
    {
        if (isMoving) return;

        Vector2 input = GetInput4Dir();
        if (input == Vector2.zero) return;

        SetDirectionBools(input);

        Vector2 step = GetStepFromDir(input);
        float distance = step.magnitude;

        if (IsBlocked(input, distance)) return;

        Vector2 target = (Vector2)transform.position + step;
        StartCoroutine(MoveTo(target));
    }

    private Vector2 GetInput4Dir()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(x) > Mathf.Abs(y)) y = 0;
        else x = 0;

        if (x != 0) x = Mathf.Sign(x);
        if (y != 0) y = Mathf.Sign(y);

        return new Vector2(x, y);
    }

    private Vector2 GetStepFromDir(Vector2 dir)
    {
        if (dir.x != 0f) return new Vector2(dir.x * cellStepX, 0f);
        return new Vector2(0f, dir.y * cellStepY);
    }

    private void SetDirectionBools(Vector2 dir)
    {
        anim.SetBool("left?", dir.x < 0);
        anim.SetBool("right?", dir.x > 0);
        anim.SetBool("up?", dir.y > 0);
        anim.SetBool("down?", dir.y < 0);
    }

    private bool IsBlocked(Vector2 dir, float distance)
    {
        Vector2 origin = col.bounds.center;
        Vector2 size = (Vector2)col.bounds.size - Vector2.one * skin;

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, dir, distance, obstacleMask);
        return hit.collider != null;
    }

    private IEnumerator MoveTo(Vector2 target)
    {
        isMoving = true;
        anim.SetBool("move?", true); 

        while ((target - (Vector2)transform.position).sqrMagnitude > 0.0001f)
        {
            Vector2 next = Vector2.MoveTowards((Vector2)transform.position, target, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(next.x, next.y, fixedZ);
            yield return null;
        }

        transform.position = new Vector3(target.x, target.y, fixedZ);

        anim.SetBool("move?", false); 
        isMoving = false;
    }
}
