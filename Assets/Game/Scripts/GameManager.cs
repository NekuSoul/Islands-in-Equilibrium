using Assets.Game.Code;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public Level Level;
        public Text ForestCountText;
        public Text CityCountText;

        // Use this for initialization
        public void Start()
        {
            Level.LoadLevel();
        }

        // Update is called once per frame
        public void Update()
        {

        }

        public void Play()
        {
            Level.AdvanceStep();
            ForestCountText.text = Level.ForestCount.ToString();
            CityCountText.text = Level.CityCount.ToString();
        }

        public void Reset()
        {
            Level.LoadLevel();
        }
    }
}
