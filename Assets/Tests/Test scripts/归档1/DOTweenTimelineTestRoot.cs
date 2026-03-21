using Dott;
using UnityEngine;

public class DOTweenTimelineTestRoot : MonoBehaviour
{
    [SerializeField] private DOTweenTimeline timeline;
    [SerializeField] private BoxCollider2D startArea;
    [SerializeField] private BoxCollider2D endArea;
    [SerializeField] private Transform actorsRoot;

    public DOTweenTimeline Timeline => timeline;
    public BoxCollider2D StartArea => startArea;
    public BoxCollider2D EndArea => endArea;
    public Transform ActorsRoot => actorsRoot;

    public void Assign(DOTweenTimeline timeline, BoxCollider2D startArea, BoxCollider2D endArea, Transform actorsRoot)
    {
        this.timeline = timeline;
        this.startArea = startArea;
        this.endArea = endArea;
        this.actorsRoot = actorsRoot;
    }
}
