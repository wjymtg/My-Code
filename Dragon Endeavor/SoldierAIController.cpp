// Fill out your copyright notice in the Description page of Project Settings.

#include "DragonEndeavors.h"
#include "SoldierAIController.h"
#include "Soldier.h"

ASoldierAIController::ASoldierAIController() {
	
}

void ASoldierAIController::BeginPlay() {
	Super::BeginPlay();
	state = Start;
}

void ASoldierAIController::Tick(float DeltaSeconds) {
	Super::Tick(DeltaSeconds);
	// If dragon within guardian range, chase, or run
	// TODO start chasing
	FVector enemyloc = UGameplayStatics::GetPlayerPawn(this, 0)->GetActorLocation();
	FVector myloc = GetPawn()->GetActorLocation();
	if (FVector::Dist(enemyloc, myloc) <= GuardianRange) {
		float hp = (Cast<ASoldier>(GetPawn()))->GetHealth();
		// Have enough hp, chase
		if (hp > (hp / 2)) {
			state = Chase;
			MoveToActor(UGameplayStatics::GetPlayerPawn(this, 0));
		}
		// Low hp, run
		else {
			state = Run;
			StartRun(UGameplayStatics::GetPlayerPawn(this, 0)->GetActorLocation());
		}
		return;
	}
	// Other situations when out of guardian range
	if (state == Start) {
		state = Wandering;
		if (FMath::FRandRange(0, 10) > 4.0f) {
			Wander();
			//If got stuck, reset to start state
			GetWorldTimerManager().SetTimer(StopHandle, this, &ASoldierAIController::Stop, 0.5f);
		}
		else {
			GetWorldTimerManager().SetTimer(StopHandle, this, &ASoldierAIController::Stop, 1.0f);
		}
	}
	else if (state == Chase) {
		// Within atk range
		if (FVector::Dist(enemyloc, myloc) <= AttackRange) {
			(Cast<ASoldier>(GetPawn()))->StartAttack();
			state = Attack;
		}
	}
	else if (state == Attack) {
		// Out of atk range
		if (FVector::Dist(enemyloc, myloc) > AttackRange) {
			MoveToActor(UGameplayStatics::GetPlayerPawn(this, 0));
			state = Chase;
		}
	}
	else if (state == Run) {
		// Dragon out of range, stop running
		if (FVector::Dist(enemyloc, myloc) > GuardianRange) {
			Stop();
		}
	}
}

void ASoldierAIController::OnMoveCompleted(FAIRequestID RequestID, EPathFollowingResult::Type Result) {
	
	if (Result == EPathFollowingResult::Success) {
		state = Start;
		// Didn't get stuck so pause the reset timer
		GetWorldTimerManager().PauseTimer(StopHandle);
	}
}

void ASoldierAIController::whenIsBurnt(AActor* damageCauser) {
	state = Run;
	StartRun(damageCauser->GetActorLocation());
}


void ASoldierAIController::StartRun(FVector away)
{
	if (GetPawn()) {
		
		FVector dir = GetPawn()->GetActorLocation() - away;
		dir.Normalize();
		FVector direction = FVector(dir.X, dir.Y, 0);
		FVector dest = GetPawn()->GetActorLocation() + direction * 500.0f;
		MoveToLocation(dest);
	}
}

void ASoldierAIController::Stop()
{
	state = Start;
}

void ASoldierAIController::Wander()
{
	if (GetPawn()) {
		FVector loc = GetPawn()->GetActorLocation();
		FVector dirVector;
		dirVector.X = FMath::FRandRange(-30, 30);
		dirVector.Y = FMath::FRandRange(-30, 30);
		dirVector.Z = FMath::FRandRange(-30, 30);
		FVector dest = loc + dirVector;
		MoveToLocation(dest);
		state = Wandering;
	}
}
