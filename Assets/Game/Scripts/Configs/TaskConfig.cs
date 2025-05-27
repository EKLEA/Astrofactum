using UnityEngine;
[CreateAssetMenu(fileName = "LevelTask", menuName = "Configs/Level Task")]
public class TaskConfig:ScriptableObject
{
    public int score;
    public ItemStack[] taskItems;
}