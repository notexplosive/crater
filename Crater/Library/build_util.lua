local build_helpers = {}

function build_helpers.removeDebugSymbols(directory)
    for _, file in ipairs(files.list(directory, true, "pdb")) do
        files.delete(file)
    end
end

return build_helpers