fireIndex = {}
fireTimes = {}
initialized = {}
anglePer = {}
numBullets = 1
right = true

function update(pattern, id, deltaTime)
	if(initialized[id] == nil) then
		init(pattern, id)
	end
	if(fireTimes[id] == nil) then
		fireTimes[id] = pattern.GetFireTimes(4.66666, 0)
	end

	if(pattern.GetStageTime() >= fireTimes[id][fireIndex[id]]) then
		fireIndex[id] = fireIndex[id] + 1
		right = not right
		
		for i=0, numBullets-1, 1 do
			bullet = pattern.NewBullet()
			bullet.speed = 4 + pattern.Math().RandomRange(0, 2)
			--bullet.speedMultiplier = .997
			bullet.type = "capsule"
			bullet.scale = 1.5
			if right then
				bullet.angle = 180
				bullet.x = pattern.StageWidth() * .55
			bullet.material = "darkRed"
			else
				bullet.angle = 0
				bullet.x = -pattern.StageWidth() * .55
			bullet.material = "orange"
			end
			bullet.y = pattern.Math().RandomRange(-pattern.StageHeight() / 2, pattern.StageHeight() / 2)
			pattern.SpawnBullet(bullet)
		end
	end
end

function init(pattern, id)
	initialized[id] = true
	fireIndex[id] = 0
end