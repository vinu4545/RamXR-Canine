using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneChange : MonoBehaviour
{
    // Matches "Scenes/Dog/Skeleton"
    public void LoadSkeletonScene()
    {
        SceneManager.LoadScene("Skeleton"); 
    }

    // Matches "Scenes/Dog/Muscle"
    public void LoadmusclesScene()
    {
        SceneManager.LoadScene("Muscle"); 
    }

    // Matches "Scenes/Dog/Testing"
    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene("Testing");
    }


 public void LoadNervesScene()
    {
        SceneManager.LoadScene("Nerves"); 
    }
}