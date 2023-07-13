using UnityEngine;
using System.Collections;
public interface GeneralStateComponent {
    public bool IsFighting();
    public bool IsWounded();    
    public bool HasFallen();
    public bool IsVisible(GameObject other, float viewAngle);
    public bool CanFight();
    public void StartFight(GameObject other, bool isStarter);
    public float TimeSinceLastFight();
    public void AddDamage(float amount);
    public float GetDamage();
    public void Heal();
    public bool IsShopper();

    public IEnumerator WaitAWhile(int seconds);
    
}
