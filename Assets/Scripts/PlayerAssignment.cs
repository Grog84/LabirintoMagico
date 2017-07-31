using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAssignment", menuName = "Personal Tools/Player Assignement", order = 3)]
public class PlayerAssignment : ScriptableObject {

    public Texture2D[] playersTextures;
    public RuntimeAnimatorController[] playersAnimators;

    public Sprite GetSprite(int playerNbr)
    {
        Sprite mySprite = Sprite.Create(playersTextures[playerNbr - 1],
                                        new Rect(0, 0, playersTextures[playerNbr-1].width,
                                        playersTextures[playerNbr - 1].height),
                                        new Vector2(0.5f, 0.5f));

        return mySprite;
    }

    public RuntimeAnimatorController GetAnimator(int playerNbr)
    {
        return playersAnimators[playerNbr - 1];
    }


}
