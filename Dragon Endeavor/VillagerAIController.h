// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "AIController.h"
#include "VillagerAIController.generated.h"

/**
 * 
 */
UCLASS()
class DRAGONENDEAVORS_API AVillagerAIController : public AAIController
{
	GENERATED_BODY()
public:
    AVillagerAIController();
    void BeginPlay() override;
    void Tick(float DeltaSeconds) override;
    void whenIsBurnt(AActor* damageCauser);
    
	// Run
	void StartRun(FVector away);
	void Stop();
	// Wander
	void Wander();

	void OnMoveCompleted(FAIRequestID RequestID, EPathFollowingResult::Type Result) override;

private:
    enum State {Start, Run, Dead, Wandering};
    State state;
	FTimerHandle StopHandle;
	UPROPERTY(EditDefaultsOnly, Category = Damage)
		float GuardianRange = 2000.0f;
};
