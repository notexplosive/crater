local build_helpers = {
    description = "utilities to help with building projects"
}

function build_helpers.removeDebugSymbols(directory)
    for _, file in ipairs(files.list(directory, true, "pdb")) do
        files.delete(file)
    end
end

return build_helpers