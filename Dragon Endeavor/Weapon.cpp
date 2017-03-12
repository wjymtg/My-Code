// Fill out your copyright notice in the Description page of Project Settings.

#include "DragonEndeavors.h"
#include "Weapon.h"
#include "Soldier.h"
#include "Sound/SoundCue.h"
#include "Dragon.h"
#include "Particles/ParticleSystemComponent.h"

AWeapon::AWeapon()
{
	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	WeaponMesh = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("WeaponMesh"));
	RootComponent = WeaponMesh;

	FireRate = 0.1f;
	WeaponRange = 10000.0f;
}

// Called when the game starts or when spawned
void AWeapon::BeginPlay()
{
	Super::BeginPlay();

}

// Called every frame
void AWeapon::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

void AWeapon::OnStartFire()
{
	//GetWorldTimerManager().SetTimer(WeaponTimer, this, &AWeapon::WeaponTrace, FireRate, true);

	if (FireLoopSound)
		FireAC = PlayWeaponSound(FireLoopSound);
	if (MuzzleFX)
		MuzzlePSC = UGameplayStatics::SpawnEmitterAttached(MuzzleFX, WeaponMesh, TEXT("MuzzleFlashSocket"));
}

void AWeapon::OnStopFire()
{
	GetWorldTimerManager().ClearTimer(WeaponTimer);

	if (FireAC)
		FireAC->Stop();
	if (FireFinishSound)
		PlayWeaponSound(FireFinishSound);
	if (MuzzlePSC)
		MuzzlePSC->DeactivateSystem();
}

UAudioComponent* AWeapon::PlayWeaponSound(USoundCue* Sound)
{
	UAudioComponent* AC = NULL;
	if (Sound)
	{
		AC = UGameplayStatics::SpawnSoundAttached(Sound, RootComponent);
	}
	return AC;
}

void AWeapon::WeaponTrace()
{
	static FName WeaponFireTag = FName(TEXT("WeaponTrace"));
	static FName MuzzleSocket = FName(TEXT("MuzzleFlashSocket"));
	// Start from the muzzle's position
	FVector StartPos = WeaponMesh->GetSocketLocation(MuzzleSocket);
	// Get forward vector of MyPawn
	FVector Forward = MyOwner->GetActorForwardVector();
	// Calculate end position
	FVector EndPos = StartPos + Forward * WeaponRange;

	// Perform trace to retrieve hit info
	FCollisionQueryParams TraceParams(WeaponFireTag, true, Instigator);
	TraceParams.bTraceAsyncScene = true;
	TraceParams.bReturnPhysicalMaterial = true;
	// This fires the ray and checks against all objects w/ collision
	FHitResult Hit(ForceInit);
	GetWorld()->LineTraceSingleByObjectType(Hit, StartPos, EndPos,
		FCollisionObjectQueryParams::AllObjects, TraceParams);
	// Did this hit anything?
	if (Hit.bBlockingHit)
	{
		UGameplayStatics::SpawnEmitterAtLocation(this, HitEffect, Hit.ImpactPoint);
		ADragon* Dragon = Cast<ADragon>(Hit.GetActor());
		if (Dragon)
		{
			Dragon->TakeDamage(Damage, FDamageEvent(), GetInstigatorController(), this);
		}
	}
}



