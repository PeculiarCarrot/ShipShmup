function init(pattern)
	fireIndex = 0
	numSets = 11
	spacePerSet = 360 / 11

	currentAngle = 0
	currentSpinSpeed = 0
	maxSpinSpeed = 100
	spinAcceleration = .3
	reverseSpin = true
	reverseSpinSpeed = .99
	spinningClockwise = false
	fireTimes = pattern.GetFireTimes(4.666666, 1, 12.2, 1.7)
end

function update(pattern, deltaTime)

	currentAngle = currentAngle + currentSpinSpeed * deltaTime
	if (spinningClockwise) then
		currentSpinSpeed = currentSpinSpeed + spinAcceleration * -1
	else
		currentSpinSpeed = currentSpinSpeed + spinAcceleration * 1
	end

	if(currentSpinSpeed < 0 ~= spinningClockwise) then
		currentSpinSpeed = currentSpinSpeed * reverseSpinSpeed;
	end
	if(currentSpinSpeed >= maxSpinSpeed) then
		spinningClockwise = true;
	elseif(currentSpinSpeed <= -maxSpinSpeed) then
		spinningClockwise = false;
	end

	if(pattern.GetStageTime() >= fireTimes[fireIndex]) then
		fireIndex = fireIndex + 1
		for place = 0, 360, spacePerSet do
			bullet = pattern.NewBullet()
			bullet.speed = 4
			bullet.angle = place + currentAngle
			bullet.material = "darkRed"
			pattern.SpawnBullet(bullet)
		end
	end
end