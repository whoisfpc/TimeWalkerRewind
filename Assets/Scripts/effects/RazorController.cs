using UnityEngine;

public class RazorController : MonoBehaviour
{
	public GameObject Line;
	public GameObject FXef; //激光击中物体的粒子效果
	
	private void Update()
	{
		Vector3 scale = new(0.5f, 0.5f, 0.5f);
		const float maxRayDistance = 500f;
		int layerMask = (1 << LayerMask.NameToLayer("Platforms")) | (1 << LayerMask.NameToLayer("Player"));

		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward, maxRayDistance, layerMask);

		if (hit)
		{
			HandleRaycastHit(hit, ref scale);
		}
		else
		{
			HandleNoHit(maxRayDistance, ref scale);
		}

		Line.transform.localScale = scale;
	}

	private void HandleRaycastHit(RaycastHit2D hit, ref Vector3 scale)
	{
		scale.y = hit.distance;

		// 设置激光击中效果的位置和显示
		FXef.transform.position = hit.point;
		FXef.SetActive(true);

		// 如果击中玩家则造成伤害
		if (hit.collider.CompareTag("Player"))
		{
			hit.collider.GetComponent<PlayerController>().TakeDamage(20, Vector3.zero);
		}
	}

	private void HandleNoHit(float distance, ref Vector3 scale)
	{
		scale.y = distance;
		FXef.SetActive(false);
	}
}