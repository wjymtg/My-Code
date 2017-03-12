// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "GameFramework/Character.h"
#include "Weapon.h"
#include "Soldier.generated.h"

UCLASS()
class DRAGONENDEAVORS_API ASoldier : public ACharacter
{
	GENERATED_BODY()

public:
	// Sets default values for this character's properties
	ASoldier();

	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	
	// Called every frame
	virtual void Tick( float DeltaSeconds ) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* InputComponent) override;

	// Fire
	void OnStartFire();
	void OnStopFire();

	// Attack
	void StartAttack();
	void StopAttack();

	// Damage
	float TakeDamage(float Damage, FDamageEvent const& DamageEvent, AController* EventInstigator, AActor* DamageCauser) override;

	// Getter
	float GetHealth() { return Health; }
protected:
	void OnDeath();
	void DoDamage();

	UPROPERTY(EditAnywhere, Category = Weapon)
		TSubclassOf<class AWeapon> WeaponClass;

	UPROPERTY(EditAnywhere, Category = Damage)
		float Health = 100.0f;
	UPROPERTY(EditDefaultsOnly)
		class UAnimMontage* DeathAnim;

	FTimerHandle DeathHandle;
	FTimerHandle AttackHandle;

private:
	class AWeapon* MyWeapon;


	
};
