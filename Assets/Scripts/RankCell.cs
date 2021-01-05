using TMPro;
using UnityEngine;

public class RankCell : MonoBehaviour
{
    public TextMeshProUGUI rankText, nameValueText, nameUnitText, scoreText;

    public void Init(int rank, string name, int score)
    {
        rankText.SetText(rank.ToString());
        nameValueText.SetText(name);
        scoreText.SetText(score.ToString());

#if OJU_ATSUMARU
        nameValueText.fontSize = 20;
        nameUnitText.gameObject.SetActive(true);
#else
        nameValueText.fontSize = 40;
        nameUnitText.gameObject.SetActive(false);
#endif

        gameObject.SetActive(true);
    }
}
