// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "GameFramework/Actor.h"

#include "Weapon.generated.h"


UCLASS()
class DRAGONENDEAVORS_API AWeapon : public AActor
{
	GENERATED_BODY()
	
public:	
public:
	// Sets default values for this actor's properties
	AWeapon();

	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

	// Called every frame
	virtual void Tick(float DeltaSeconds) override;

	// Getter
	USkeletalMeshComponent* GetWeaponMesh() { return WeaponMesh; }

	// Fire
	void OnStartFire();
	void OnStopFire();

	UAudioComponent* PlayWeaponSound(class USoundCue* Sound);

	// Setter & Getter
	void SetWeaponOwner(class ASoldier* owner) { MyOwner = owner; }
	ASoldier* GetWeaponOwner() { return MyOwner; }
	float GetDamage() { return Damage; }
	float GetFireRate() { return FireRate; }

	void WeaponTrace();
protected:
	UPROPERTY(VisibleDefaultsOnly, BlueprintReadOnly, Category = Weapon)
		USkeletalMeshComponent* WeaponMesh;

	UPROPERTY(EditDefaultsOnly, Category = Sound)
		class USoundCue* FireLoopSound;
	UPROPERTY(EditDefaultsOnly, Category = Sound)
		class USoundCue* FireFinishSound;
	UPROPERTY(Transient)
		class UAudioComponent* FireAC;

	UPROPERTY(EditDefaultsOnly, Category = Effects)
		class UParticleSystem* MuzzleFX;
	UPROPERTY(Transient)
		class UParticleSystemComponent* MuzzlePSC;


	UPROPERTY(EditAnywhere, Category = Weapon)
		float FireRate;
	UPROPERTY(EditAnywhere, Category = Weapon)
		float WeaponRange;
	UPROPERTY(EditDefaultsOnly, Category = Effects)
		class UParticleSystem* HitEffect;
	UPROPERTY(EditAnywhere, Category = Weapon)
		float Damage = 2.0f;

	class ASoldier* MyOwner;

	FTimerHandle WeaponTimer;

	
	
};
