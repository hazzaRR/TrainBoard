import json
duplicate_crs = {}
with open("./national-rail-stations.json") as json_file:
    stations = json.load(json_file)
    for station in stations:
        exists = duplicate_crs.get(station['crs'])
        if exists is None:
            duplicate_crs[station['crs']] = 1
        else:
            duplicate_crs[station['crs']] += 1


for key, value in duplicate_crs.items():
    if value > 1:
        print(str(key) + " " + str(value))