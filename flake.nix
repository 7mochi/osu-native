{
  description = "osu-native development environment";

  inputs.nixpkgs.url = "github:nixos/nixpkgs/nixpkgs-unstable";

  outputs =
    { nixpkgs, ... }:
    let
      system = "x86_64-linux";
      pkgs = import nixpkgs { inherit system; };
    in
    {
      devShells.${system}.default = pkgs.mkShell {
        packages = with pkgs; [
          dotnet-sdk_10
          clang
          zlib
          just
        ];

        shellHook = ''
          echo "osu-native"
          echo "  just test      (dotnet test -c Release)"
          echo "  just fmt       (dotnet format)"
        '';
      };
    };
}
