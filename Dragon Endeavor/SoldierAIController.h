// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "AIController.h"
#include "SoldierAIController.generated.h"

/**
 * 
 */
UCLASS()
class DRAGONENDEAVORS_API ASoldierAIController : public AAIController
{
	GENERATED_BODY()
public:
	ASoldierAIController();
	void BeginPlay() override;
	void Tick(float DeltaSeconds) override;
	void OnMoveCompleted(FAIRequestID RequestID, EPathFollowingResult::Type Result) override;

	void whenIsBurnt(AActor* damageCauser);

	// Run
	void StartRun(FVector away);
	void Stop();
	// Wander
	void Wander();

protected:
	enum State { Start, Chase, Attack, Dead, Wandering, Run};
	State state;
	// Ranges
	UPROPERTY(EditDefaultsOnly, Category = Damage)
		float AttackRange = 500.0f;
	UPROPERTY(EditDefaultsOnly, Category = Damage)
		float GuardianRange = 2000.0f;


	FTimerHandle StopHandle;

	
};
