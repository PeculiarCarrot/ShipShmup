
delayTimer = 0
delayTime = 1

function update(pattern, deltaTime)
	if delayTimer >= delayTime then
		bullet = pattern.NewBullet()
		pattern.SpawnBullet(bullet)
		delayTimer = 0
	end
	delayTimer = delayTimer + deltaTime
end

function init(pattern)
	
end