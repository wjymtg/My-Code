// Fill out your copyright notice in the Description page of Project Settings.

#include "DragonEndeavors.h"
#include "VillagerAIController.h"
#include "Villager.h"

AVillagerAIController::AVillagerAIController(){
    
}

void AVillagerAIController::BeginPlay(){
    Super::BeginPlay();
    state = Start;
}

void AVillagerAIController::whenIsBurnt(AActor* damageCauser){
    state = Run;
	StartRun(damageCauser->GetActorLocation());
}

void AVillagerAIController::Tick(float DeltaSeconds){
    Super::Tick(DeltaSeconds);
	//Run away from dragon
	FVector enemyloc = UGameplayStatics::GetPlayerPawn(this, 0)->GetActorLocation();
	FVector myloc = GetPawn()->GetActorLocation();
	if (FVector::Dist(enemyloc, myloc) <= GuardianRange) {
		state = Run;
		StartRun(UGameplayStatics::GetPlayerPawn(this, 0)->GetActorLocation());
		return;
	}
    if(state == Start){
		state = Wandering;
		if (FMath::FRandRange(0,10) > 4.0f) {
			Wander();
			//If got stuck, reset to start state
			GetWorldTimerManager().SetTimer(StopHandle, this, &AVillagerAIController::Stop, 0.5f);
		}
		else {
			GetWorldTimerManager().SetTimer(StopHandle, this, &AVillagerAIController::Stop, 1.0f);
		}
    }
}

void AVillagerAIController::Stop() {
	state = Start;
}

void AVillagerAIController::StartRun(FVector away) {
	if (GetPawn()) {
		(Cast<AVillager>(GetPawn()))->setSpeed(800.0f);
		FVector dir = GetPawn()->GetActorLocation() - away;
		dir.Normalize();
		FVector direction = FVector(dir.X, dir.Y, 0);
		FVector dest = GetPawn()->GetActorLocation() + direction * 500.0f;
		MoveToLocation(dest);
	}

}

void AVillagerAIController::OnMoveCompleted(FAIRequestID RequestID, EPathFollowingResult::Type Result)
{
	if (Result == EPathFollowingResult::Success) {
		state = Start;
		// Didn't get stuck so pause the reset timer
		GetWorldTimerManager().PauseTimer(StopHandle);
	}
}


void AVillagerAIController::Wander() {
	if (GetPawn()) {
		FVector loc = GetPawn()->GetActorLocation();
		FVector dirVector;
		dirVector.X = FMath::FRandRange(-200, 200);
		dirVector.Y = FMath::FRandRange(-200, 200);
		dirVector.Z = FMath::FRandRange(-200, 200);
		FVector dest = loc + dirVector;
		MoveToLocation(dest);
		state = Wandering;
	}
}