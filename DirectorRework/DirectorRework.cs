using BepInEx;
using RoR2;
using UnityEngine;

namespace DirectorRework
{
  [BepInPlugin("com.Nuxlar.DirectorRework", "DirectorRework", "1.0.1")]

  public class DirectorRework : BaseUnityPlugin
  {

    public void Awake()
    {
      On.RoR2.CombatDirector.Simulate += ResetMonsterCard;
    }

    private void ResetMonsterCard(On.RoR2.CombatDirector.orig_Simulate orig, CombatDirector self, float deltaTime)
    {
      if (self.targetPlayers)
      {
        self.playerRetargetTimer -= deltaTime;
        if (self.playerRetargetTimer <= 0.0)
        {
          self.playerRetargetTimer = self.rng.RangeFloat(1f, 10f);
          self.PickPlayerAsSpawnTarget();
        }
      }
      self.monsterSpawnTimer -= deltaTime;
      if ((double)self.monsterSpawnTimer > 0.0)
        return;
      if (self.AttemptSpawnOnTarget((bool)self.currentSpawnTarget ? self.currentSpawnTarget.transform : null))
      {
        if (self.shouldSpawnOneWave)
        {
          Debug.Log((object)"CombatDirector hasStartedwave = true");
          self.hasStartedWave = true;
        }
        self.monsterSpawnTimer += self.rng.RangeFloat(self.minSeriesSpawnInterval, self.maxSeriesSpawnInterval);

        if (self.currentMonsterCard.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab.GetComponent<CharacterBody>().isChampion)
        {
          self.SetNextSpawnAsBoss();
        }
        else
          self.currentMonsterCard = null;

        self.ResetEliteType();
      }
      else
      {
        self.monsterSpawnTimer += self.rng.RangeFloat(self.minRerollSpawnInterval, self.maxRerollSpawnInterval);
        if (self.resetMonsterCardIfFailed)
          self.currentMonsterCard = null;
        if (!self.shouldSpawnOneWave || !self.hasStartedWave)
          return;
        Debug.Log("CombatDirector wave complete");
        self.enabled = false;
      }
    }

  }
}