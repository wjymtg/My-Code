// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "GameFramework/Character.h"
#include "Villager.generated.h"

UCLASS()
class DRAGONENDEAVORS_API AVillager : public ACharacter
{
    GENERATED_BODY()
    
public:
    // Sets default values for this character's properties
    AVillager();
    
    // Called when the game starts or when spawned
    virtual void BeginPlay() override;
    
    // Called every frame
    virtual void Tick( float DeltaSeconds ) override;
    
    // Called to bind functionality to input
    virtual void SetupPlayerInputComponent(class UInputComponent* InputComponent) override;
    
	void setSpeed(float speed);
    
	void Wander();

    // Damage
    float TakeDamage(float Damage, FDamageEvent const& DamageEvent, AController* EventInstigator, AActor* DamageCauser) override;
    
protected:
    void OnDeath();
    UPROPERTY(EditDefaultsOnly)
    class UAnimMontage* DeathAnim;
    UPROPERTY(EditAnywhere, Category = Health)
    float Health = 100.0f;
    
    FTimerHandle RunHandle;
    FTimerHandle DeathHandle;
	FTimerHandle WanderHandle;
};

