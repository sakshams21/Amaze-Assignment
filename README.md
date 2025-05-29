# Light&Heavy Attack

## 🎮 Gameplay Programmer Test – AI Combat System

**Objective:**  
Develop an AI system in Unity Engine where enemies respond differently to light and heavy attacks:

- **Light Attack:** Shrinks the enemy.
- **Heavy Attack:** Triggers a blood splatter, spawns a new AI at the splatter location (which behaves like the original).

---
## 📹 Gameplay Demo

> 📺 [Gameplay Demo/Light&Heavy Gameplay Demo.mp4](#)  
*(Add link to your gameplay video on YouTube, Drive, or GitHub if available.)*

---

## 🧠 Features

### ✅ Player Character Setup
- Modied unity's 3rd Person Character asset.
- Two attack types implemented:
  - **Light Attack:** Triggers enemy scale-down logic.
  - **Heavy Attack:** Spawns a blood splatter and a new AI.

### ✅ Enemy AI Behavior
- **Light Attack:**
  - Reduces the AI's scale by 0.1 (clamped at a minimum of 0.5).
- **Heavy Attack:**
  - Spawns a **blood splatter effect** (particle or decal).
  - **Generates a new AI** at the splatter location.
  - Original AI remains active unless destroyed.

### ✅ Assets Used
- **Free assets** from the Unity Asset Store.
  - **Player**: https://assetstore.unity.com/packages/p/starter-assets-character-controllers-urp-267961 
  - **Enemy**: https://assetstore.unity.com/packages/p/01-monster-lizard-181592
  - **Blood Splatter**: https://assetstore.unity.com/packages/p/blood-splatter-decal-package-7518
  - **DoTween**: https://assetstore.unity.com/packages/p/dotween-hotween-v2-27676
  - **Player Attack Animations**: https://assetstore.unity.com/packages/p/rpg-animations-pack-free-288783
  - **Dissolve**: https://assetstore.unity.com/packages/p/urp-dissolve-2020-191256

## 🛠️ Code Breakdown & Logic Breakdown

### 👤 Script Overview
- Object Pooling used for enemy spwan.(EnemyPoolManager.cs)
- Central Event Invoker and handler. (EventManager.cs)
- Audio for Hit,Spawn,Death. (AudioManager.cs)
- Singleton which handles EnemySpawn process. (GameManager.cs)
- Weapon script which handles the collision trigger detection. (weapon.cs)

### 👤 Logic
- Weapon has the collider and Rigidbody for collion Detection.
- Event is fired for Hit (for visual  and audio feedback)
- GameManager has variable which decides which attack is happening currently(light or heavy).
- Based on the type of Attack it GameManager handles what to happen afterwards.
- Either to only reduce size or spawn blood splatter and spawn enemies after a delay.
- After Enemy's death to animate Dissolve effect used Coroutine and Property Block of the Material instead of using Renderer.material which will create a new instance of material which is not performant.
- At the end it triggers DeathEvent which GameManager uses for total count of alive enemies and Audio Manager to play death sound.
- ```csharp
	public void TriggerDissolve()
    {
      StartCoroutine(AfterDeath_Coro());
    }

    IEnumerator AfterDeath_Coro()
    {
      foreach (var item in Colliders)
      {
        item.enabled = false;
      }


      float duration = 1f;
      float startValue = 0;
      float targetValue = 1f;

      float elapsedTime = 0f;
      while (elapsedTime < duration)
      {
        elapsedTime += Time.deltaTime;
        float currentValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);

        _mpb.SetFloat(DissolvePropertyID, currentValue);
        This_MeshRenderer.SetPropertyBlock(_mpb);

        yield return null;
    }


      _mpb.SetFloat(DissolvePropertyID, targetValue);
      This_MeshRenderer.SetPropertyBlock(_mpb);

      //backtopool
      EventManager.TriggerEnemyDeathEvent(this);
    }
- Blood Splatter location is generated based on mentioned code below mainly on right side of the camera as heavy animation swings to the right.
- ```csharp
  private void SecondaryAttackHit()
  {
    Vector3 cameraRight = Camera.main.transform.right;
    Vector3 randomOffset = Random.onUnitSphere * 2.5f;

    if (Vector3.Dot(randomOffset, cameraRight) < 0)
    {
        randomOffset = -randomOffset;
    }

    Vector3 pos = transform.position + randomOffset;
    pos.y = 0;

    GameManager.Instance.SpawnEnemy(pos);

  }
- Caching Animation names into has values and triggering this when needed.



---

## 🚀 Installation & Run Instructions

1. Ensure you have **Unity Hub** and a compatible version of **Unity.
2. This Projects uses Unity 6000.0.27f1.
3. Clone or download the project repository.
4. Open Unity Hub and click on **"Add Project"**, then navigate to the project folder.
5. Open the project in Unity.
6. Press **Play** in the editor to test gameplay features.

---

## 👤 Author

**Name:** *Saksham Kumar*  
**Email:** *sakshams21@gmail.com*  
**LinkedIn / GitHub:** *https://www.linkedin.com/in/sakshams21/*

