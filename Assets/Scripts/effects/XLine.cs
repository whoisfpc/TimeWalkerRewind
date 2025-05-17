using UnityEngine;

public class XLine : MonoBehaviour
{
	public GameObject Line;
	public GameObject FXef; //激光击中物体的粒子效果

	private void Update()
	{
		Vector3 sc = new(0.5f, 0, 0.5f); // 变换大小
		//发射射线，通过获取射线碰撞后返回的距离来变换激光模型的y轴上的值
		if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
		{
			Debug.DrawLine(transform.position, hit.point);
			sc.y = hit.distance;
			FXef.transform.position = hit.point; //让激光击中物体的粒子效果的空间位置与射线碰撞的点的空间位置保持一致；
			FXef.SetActive(true);
		}
		else
		{
			//当激光没有碰撞到物体时，让射线的长度保持为500m，并设置击中效果为不显示
			sc.y = 500;
			FXef.SetActive(false);
		}

		Line.transform.localScale = sc;
	}
}