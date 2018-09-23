using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using SubjectNerd.Utilities;

public class VerticalNavigationGroup : MonoBehaviour
{
	[Reorderable]
	public List<Selectable> selectables;

	private void OnEnable()
	{
		List<Selectable> selectables = this.selectables
			.Where(selectable => selectable.gameObject.activeInHierarchy)
			.ToList();

		for(int i = 0; i < selectables.Count; i++)
		{
			Selectable previous = i > 0 ? selectables[i - 1] : null;
			Selectable next = i < selectables.Count - 1 ? selectables[i + 1] : null;

			Navigation nav = selectables[i].navigation;
			nav.selectOnUp = previous;
			nav.selectOnDown = next;
			selectables[i].navigation = nav;
		}
	}
}
