/***********************************************************************************************************
 * JELLY CUBE - GAME STARTER KIT - Compatible with Unity 5                                                 *
 * Produced by TROPIC BLOCKS - http://www.tropicblocks.com - http://www.twitter.com/tropicblocks           *
 * Developed by Rodrigo Pegorari - http://www.rodrigopegorari.com                                          *
 ***********************************************************************************************************/

using UnityEngine;
using System.Collections;

namespace JellyCube
{
    public class Sound : MonoBehaviour
    {
        public void Start()
        {
            DontDestroyOnLoad(gameObject);

            GetComponent<AudioSource>().loop = true;
        }
    }
}