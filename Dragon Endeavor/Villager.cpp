// Fill out your copyright notice in the Description page of Project Settings.

#include "DragonEndeavors.h"
#include "Villager.h"
#include "VillagerAIController.h"

// Sets default values
AVillager::AVillager()
{
 	// Set this character to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
    AIControllerClass = AVillagerAIController::StaticClass();

}

// Called when the game starts or when spawned
void AVillager::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void AVillager::Tick( float DeltaTime )
{
	Super::Tick( DeltaTime );

}

// Called to bind functionality to input
void AVillager::SetupPlayerInputComponent(class UInputComponent* InputComponent)
{
	Super::SetupPlayerInputComponent(InputComponent);

}


float AVillager::TakeDamage(float Damage, FDamageEvent const& DamageEvent, AController* EventInstigator, AActor* DamageCauser)
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
            GetController()->PawnPendingDestroy(this);
            GetWorldTimerManager().SetTimer(DeathHandle, this, &AVillager::OnDeath, PlayAnimMontage(DeathAnim)-0.25f);
            
        }
        else{
            // Villager is burnt, start running!
            // TODO: Process run
            (Cast<AVillagerAIController>(GetController()))->whenIsBurnt(this);
        }
    }
    return ActualDamage;
}

void AVillager::OnDeath(){
    Super::Destroy();
}


void AVillager::setSpeed(float speed) {
	
}