local build_helpers = {}

function build_helpers.removeDebugSymbols()
    for _, file in ipairs(files.list(".build", true, "pdb")) do
        files.delete(file)
    end
end

return build_helpers