using System;
using System.Collections;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] Projectile projectileToSpawn; // replace with object pool
    [SerializeField] Vector3 spawnOffset;
     private ObjectPool<Projectile> projPool = new ObjectPool<Projectile>();

    private bool onCooldown;

    private void Start()
    {
        projPool.InitalizePool(projectileToSpawn, 10);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void SpawnProjectile(Vector2 moveDir, float cooldownTime)
    {
        if (onCooldown) return;
        
        Projectile proj = projPool.GetObject();
        if(proj == null) return;
        else
        {
            
            proj.gameObject.transform.position = transform.position + spawnOffset;
            //Projectile projectile = Instantiate(projectileToSpawn, transform.position + spawnOffset, Quaternion.identity).GetComponent<Projectile>(); // replace with Get() object pool
            proj.Init(moveDir, this);

            StartCoroutine(Countdown(cooldownTime, null));
        }

    }

    private void SpawnProjectileRepeating(Vector2 moveDir, float repeatRate)
    {
        print("why am i being called");
        Projectile projectile = Instantiate(projectileToSpawn, transform.position + spawnOffset, Quaternion.identity).GetComponent<Projectile>(); // replace with Get() object pool
        projectile.Init(moveDir, this);

        StartCoroutine(Countdown(repeatRate, () => SpawnProjectileRepeating(moveDir, repeatRate)));
    }

    private IEnumerator Countdown(float countdownTime, Action endAction)
    {
        onCooldown = true;

        yield return new WaitForSeconds(countdownTime);
        endAction?.Invoke();

        onCooldown = false;
    }

    public void Release(Projectile obj)
    {
        projPool.ReturnObject(obj);
    }

}
