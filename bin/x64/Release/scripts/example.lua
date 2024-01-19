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