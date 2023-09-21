import os


def regenerate_atlas():
    print("1. Regenerating atlas...")
    os.system(" ".join(["free-tex-packer-cli", "--project", ".atlas.ftpp"]))


def update_atlas_data():
    print("2. Updating atlas data...")
    with open("atlas.tpsheet") as in_file:
        data = in_file.readlines()

    beginning_lines = []
    updated_lines = []

    for line in data:
        if not len(line):
            updated_lines.append(line)
            continue

        if line[0] == ".":
            line = line.split("-", 1)[-1]
            updated_lines.append(line)
        else:
            beginning_lines.append(line)

    updated_lines.sort()

    with open("atlas.tpsheet", "w") as out_file:
        out_file.write("".join(beginning_lines))
        out_file.write("".join(updated_lines))


if __name__ == "__main__":
    regenerate_atlas()
    update_atlas_data()
    input("Press Enter to exit. ")
