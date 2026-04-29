using UnityEngine;
using UnityEngine.SceneManagement; // Allows us to switch scenes

public class sceneChange : MonoBehaviour
{
    // Function to load the Skeleton Scene
    public void LoadSkeletonScene()
    {
        SceneManager.LoadScene("SkeletonScene");
    }

    // Function to return to the Main Menu
  

    // Function to load the Muscles Scene
    public void LoadmuscleScene()
    {
        SceneManager.LoadScene("Muscle");
    }

    // Function to load the Organs Scene
    public void LoadorgansScene()
    {
        SceneManager.LoadScene("OrgansScene");
    }

    // Function to load the Nervous System Scene
    public void LoadnervousScene()
    {
        SceneManager.LoadScene("NervousScene");
    }
      public void LoadMainMenuScene()
    {
        // Make sure "Testing" is the exact name of your main scene
        SceneManager.LoadScene("Testing");
    }
}