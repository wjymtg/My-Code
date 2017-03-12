// Fill out your copyright notice in the Description page of Project Settings.

#include "DragonEndeavors.h"
#include "Soldier.h"
#include "SoldierAIController.h"


// Sets default values
ASoldier::ASoldier()
{
 	// Set this character to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
	AIControllerClass = ASoldierAIController::StaticClass();

}

// Called when the game starts or when spawned
void ASoldier::BeginPlay()
{
	Super::BeginPlay();
	if (WeaponClass)
	{
		UWorld* World = GetWorld();
		if (World)
		{
			FActorSpawnParameters SpawnParams;
			SpawnParams.Owner = this;
			SpawnParams.Instigator = Instigator;
			// Need to set rotation like this because otherwise gun points down
			FRotator Rotation(0.0f, 0.0f, -90.0f);
			// Spawn the Weapon
			MyWeapon = World->SpawnActor<AWeapon>(WeaponClass, FVector::ZeroVector,
				Rotation, SpawnParams);
			if (MyWeapon)
			{
				// This is attached to "WeaponPoint" which is defined in the skeleton
				MyWeapon->AttachToComponent(GetMesh(),
					FAttachmentTransformRules::KeepRelativeTransform,
					TEXT("WeaponPoint"));
				MyWeapon->SetWeaponOwner(this);
			}
		}
	}
	
}

// Called every frame
void ASoldier::Tick( float DeltaTime )
{
	Super::Tick( DeltaTime );

}

// Called to bind functionality to input
void ASoldier::SetupPlayerInputComponent(class UInputComponent* InputComponent)
{
	Super::SetupPlayerInputComponent(InputComponent);

}

void ASoldier::OnStartFire()
{
	if (MyWeapon)
		MyWeapon->OnStartFire();
}

void ASoldier::OnStopFire()
{
	if (MyWeapon)
		MyWeapon->OnStopFire();
}

void ASoldier::StartAttack()
{
	GetWorldTimerManager().SetTimer(AttackHandle, this, &ASoldier::DoDamage, MyWeapon->GetFireRate(), true);
	OnStartFire();
}

void ASoldier::StopAttack()
{
	GetWorldTimerManager().ClearTimer(AttackHandle);
	OnStopFire();
}



float ASoldier::TakeDamage(float Damage, FDamageEvent const & DamageEvent, AController * EventInstigator, AActor * DamageCauser)
{
	float ActualDamage = Super::TakeDamage(Damage, DamageEvent,
		EventInstigator, DamageCauser);
	if (ActualDamage > 0.0f)
	{
		Health -= ActualDamage;
		if (Health <= 0.0f)
		{
			// We're dead, don't allow further damage
			bCanBeDamaged = false;
			// TODO: Process death
			if (this->GetMesh())
				this->GetMesh()->Deactivate();
			GetController()->PawnPendingDestroy(this);
			GetWorldTimerManager().SetTimer(DeathHandle, this, &ASoldier::OnDeath, PlayAnimMontage(DeathAnim)-0.25f);

		}
		else {
			// Soldier is burnt
			// TODO: Process run
			(Cast<ASoldierAIController>(GetController()))->whenIsBurnt(this);
		}
	}
	return ActualDamage;
}

void ASoldier::OnDeath()
{
	Super::Destroy();
}

void ASoldier::DoDamage() {
	UGameplayStatics::GetPlayerPawn(this, 0)->TakeDamage(MyWeapon->GetDamage(), FDamageEvent(), GetInstigatorController(), this);
}