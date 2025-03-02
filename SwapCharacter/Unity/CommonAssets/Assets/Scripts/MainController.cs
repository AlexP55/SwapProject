using System.Collections;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public ClickController Selected;

    public void Select(ClickController controller)
    {
        if (Selected == null)
        {
            controller.Select();
        }
        else
        {
            if (Selected == controller)
            {
                controller.UnSelect();
            }
            else
            {
                // change positions
                Selected.GetComponent<ClickController>().Move(controller.transform.localPosition);
                controller.GetComponent<ClickController>().Move(Selected.transform.localPosition);

                StartCoroutine(StopCoroutine(Selected, controller));
                Selected.UnSelect();
                controller.UnSelect();
            }
        }
    }

    private IEnumerator StopCoroutine(ClickController controller1, ClickController controller2)
    {
        var time = 0f;
        while (time < 2f && (controller1.moving || controller2.moving))
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        controller1.Stop();
        controller2.Stop();
        int ind1 = controller1.transform.GetSiblingIndex();
        int ind2 = controller2.transform.GetSiblingIndex();
        controller1.transform.SetSiblingIndex(ind2);
        controller2.transform.SetSiblingIndex(ind1);
        yield return new WaitForEndOfFrame();
        yield break;
    }
}
