Callbacks:Add("OnModuleProcessing", function(module_filename, is_trusted)
    if is_trusted and module_filename == "C:\\Windows\\user32.dll" then 
        return false -- if the checked filename is equal to user32.dll's path then make it flag it 
    end 

    return is_trusted
end)

Callbacks:Add("OnStringProcessing", function(string, lenght, address, is_suspicious)
    if string == "redengine" then 
        return true
    end 

    return is_suspicious
end)

Callbacks:Add("OnModProcessing", function(mod, is_suspicious)
    if mod.szName == "example.rpf" then 
        return true 
    end 

    return is_suspicious
end)

Callbacks:Add("OnPrefetchProcessing", function(file, is_suspicious)
    if file.szHash == "blacklistedhash" then 
        return true 
    end 

    return is_suspicious
end)

Console.Log("sample log")

if FiveM.Exists() then 
    local modules = FiveM.GetModules() 
    local strings = FiveM.GetStrings()

    for i=0, modules.Count do 
        local module = modules[i]

        Console.Log(module.ModuleName)
    end 
end 

local files = CPrefetch.GetFiles()
for i=0, files.Count do 
    local file = files[i]

    local success = file.bSuccess

    if success then 
        local path = file.szPath
        local exec_name = file.szExecutableName
        local hash = file.szHash
        local filenames_used = file.szFileNames
    end 
end 

local mods = Mods.Get()

for i=0, mods.Count do 
    local mod = mods[i]

    local path = mod.szPath
    local name = mod.szName
end 

String.Get(byte_array_here)