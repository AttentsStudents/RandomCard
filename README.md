# RandomCardUnityTeamProject

팀장: 조정우 => 기획
팀원: 이동민 => 기획, 디자인
천지운 => 프로그래밍, 디자인

작업 스케줄 (Atlassian - Jira): https://randomcardattents.atlassian.net/jira/software/projects/SCRUM/boards/1



**기본적으로 develop 브랜치에서 작업 후 반영하고 테스트를 완료 후 main 브랜치로 반영



**git 저장소에서 최신버전 불러오기(작업 전 무조건 먼저 해줄 것!!!! 동기화 문제 & 병합(merge)시 코드 충돌 방지)

git pull (서버주소) (불러올 브랜치) : ex) git pull origin develop




**git에 작업사항 반영하기

작업하기 => (선택) git status : 어떤 파일이 바뀌었는지 확인 가능

=> git add . : 모든 경로에 바뀐 파일 적용

=> git commit -m (커밋메시지) : ex) git commit -m "추가: 전투 애니메이션"

=> git push (서버주소) (올릴 브랜치) : ex) git push origin develop 



**git 리포지토리 서버 주소 확인하는법

git remote -v



**리포지토리 서버 별명 추가

git add (별명) (서버주소)
ex) git remote add origin "https://github~~~~~"
