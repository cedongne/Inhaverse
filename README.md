
# Inhaverse
인하대학교 컴퓨터공학과 2021-2 컴퓨터공학종합설계 프로젝트

INHA Univ. 2021-2 CSE Capstone design : Metaverse project




- # 서론
  -	배경
COVID-19 출현 이후 사태가 악화되며 오프라인 수업이 제한되고 야외 활동이 어려워진 시국에, 학우들은 Zoom과 같은 단순 화상 회의 시스템만으로 모든 교내 활동을 해결하고 있다. 하지만 형체 없는 만남과 모임으로 서로의 유대감을 쌓는 것은 한계가 있었고, 이로 인해 아바타를 활용한 가상 공간에서의 만남을 이룰 수 있는 Roblox, ZEPETO, ifland와 같은 메타버스 시스템이 큰 주목을 받게 되었다.

  -	기대효과
우리 Inhaverse는 이러한 메타버스 시스템을 인하대학교 용현캠퍼스에 적용하여 학우들이 실제 캠퍼스에서 다른 학우들과 만나 소통하는 듯한 대리만족을 느낄 수 있게 하고, 교내 DB 시스템과 연동하였을 때 E-learning 시스템을 효과적으로 이용할 수 있다. 또한 포스트 코로나에서도 병행될 비대면 콘텐츠를 효과적으로 진행하는 시스템이 될 것이라는 기대효과를 가지고 있다.

- # 시스템 구조
 
   <p align="center"><img src = "https://images.velog.io/images/cedongne/post/5b4289a2-55fe-4999-be03-bc75e03fe748/image.png"></p>
   
  Inhaverse는 Unity Engine으로 클라이언트 개발을 하였으며, Photon 솔루션에서 제공하는 다양한 서비스 중 On-premise 방식의 Photon server를 선택하여 시스템 서버를 구축하였다. 데이터베이스는 게임 시스템에 적합한 PlayFab, 화상 통신을 위한 웹 서버는 배포된 Owake 시스템을 활용하였다.

   해당 장에선 이후에 보여질 전체적인 Inhaverse 시스템 아키텍처가 어떻게 구성되어 있는지 흐름도와 같은 시각적 도구를 활용하여 표현하고, 각 아키텍처의 상세 모듈에 대해 다룬다.
 
  -  시스템 아키텍처
  
     <p align="center"><img src = "https://images.velog.io/images/cedongne/post/0a269f17-be7d-4022-84a7-4ab5ff6d4c6d/image.png"></p>
     해당 시스템의 구성요소는 서버, 클라이언트, DB로 이루어져 있으며, 각 구성요소는 역할에 따라 특정한 모듈을 사용한다.
   
  - 클라이언트
    - 애니메이션
  서버에 연결된 플레이어는 애니메이션이 항시 동작한다. 아래는 각 플레이어 오브젝트에 공통으로 할당된 Animator controller component로, 아래 그림과 같은 로직으로 플레이어의 애니메이션을 실행한다.
  
      <p align="center"><img src = "https://images.velog.io/images/cedongne/post/6395d754-01b2-4589-aecc-6b6f569989c1/image.png"></p>
      
    - 텍스트 및 음성 채팅
       월드에 접속한 후 Photon Chat, Photon Voice2 패키지를 Photon PUN2와 연동하여 텍스트 및 음성 채팅 기능을 이용할 수 있다.
       
       서버당 한 룸에는 하나의 채팅 채널이 존재하고 모든 클라이언트는 각자의 챗 클라이언트를 가진다. 텍스트 채팅은 플레이어 자신이 속한 채팅 채널에 있는 플레이어들과, 음성 채팅은 Photon Voice2 에셋에서 제공하는 Voice connection, Recorder, Speaker 컴포넌트를 통해 같은 채널(Interest group) 내의 플레이어들과 주고받는다.
     - DB
      
       <p align="center"><img src = "https://images.velog.io/images/cedongne/post/fdb99fe4-6d60-45ac-abe3-0371e134c5e0/image.png"></p>
       
       PlayFab에 String, Int, JSON 등의 형태로 저장된 Inhaverse의 데이터 정보를 데이터베이스 스키마로 만들면 위와 같은 관계로 이루어져 있다.
       
- # 시스템 모듈
  - **로그인 화면**
  
    <p align="center"><img src = "https://images.velog.io/images/cedongne/post/575203dc-bcfa-4a12-ad9b-975985269609/image.png"></p>
  
    서버에 접속하면 사용자에게 최초로 보여지는 화면이다. 이메일은 11111111@inha.edu와 같은 학교 이메일, 비밀번호, 이름을 입력하여 회원가입을 해야 하고, 가입에 성공하면 해당 정보는 PlayFab에 저장되는 동시에 이메일에서 학번을 추출해 정보로 함께 저장한다.
 
     회원가입 이후 로그인할 땐 이름 칸을 공란으로 비워 두어도 정상적으로 로그인이 가능하다. 로그인 후 두 가지 캐릭터 중 하나를 선택하여 게임을 플레이할 수 있다.

  - **HUD**
  
    <p align="center"><img src = "https://images.velog.io/images/cedongne/post/9da3fe3b-5296-4499-a59d-589341d9d691/image.png"></p>
    
     로그인 후 보이는 Head Up Display이다. 하단에 서버를 통해 채팅 할 수 있는 UI와 기본적인 기능 5가지를 UI로 만들어 제공하였다. 각각의 용도는 다음과 같다. 
     
    - Capture : HUD를 제거한 현재 인게임 화면을 캡쳐하고, 로컬 저장소에 저장할 수 있다.
    - Speaker : 자신에게 들리는 다른 플레이어의 보이스 소리를 조절할 수 있다. 거리에 따라 음성 크기가 변화하는 3D Audio 기능을 제공한다.
    - Voice : 자신의 보이스 기능을 On/Off 할 수 있다. 보이스가 활성화된 상태에서 마이크 사용이 확인되면 해당 사용자의 이름표가 초록색으로 변한다.
    - Info : 사용자 자신의 개인 정보 및 속한 수업 정보를 표시하는 창을 화면에 출력한다.
    - Option : 사용자가 유용하게 사용할 수 있는 옵션 기능을 표시하는 창을 화면에 출력한다. 옵션의 종류는 아래와 같다.
    
       |  옵션              | 설명 																				|
       | :----------------: | ----------------------------------------------------------	|
       | `조작법`            | 사용자의 커맨드 및 단축키를 알려준다. 					|
       | `로그아웃` 		  	 | 월드에서 나가 로그인 전 상태인 로비로 이동한다. |
       | `프로그램 종료`     | Inhaverse 애플리케이션을 종료한다.						|
      
    
     플레이어의 키보드와 마우스 입력은 Player Controller, Camera Controller 모듈에서 맡아 처리하며, 다음과 같은 기능을 제공한다.
   
     |  기능				| 단축키 		|  기능			| 단축키 			|  기능				| 단축키		|
     | :--------------:	| :----------:	| :----------:	| :-------------:	| :-----------:	| :----------:	|
     | 이동 				| `W,A,S,D`| 점프			| `SPACE BAR`| 걷기/달리기	| `R`			|
     | 상호작용 		| `E`			| 시점 변환	| `B`				| 정보 				| `I`			|
     | 옵션/창 닫기 	| `ESC`		| 음성 채팅	| `V` 				| 메시지 채팅	| `ENTER`	|
     | 댄스 				| `F1`			|
      
     기본적으로 마우스 커서는 보이지 않는(Locked) 상태이며, 마우스를 회전하는 것에 따라 플레이어의 시점이 회전한다. 마우스 좌클릭으로 마우스 커서를 보이게 할 수 있고, 이때 시점 회전 기능은 잠시 중단된다. 다른 클릭 이벤트가 없다면 3초 후에 마우스 커서는 자동으로 Locked 상태가 되고, 마우스 우클릭을 통해 원할 때 커서를 Locked 상태로 만들 수 있다.
     
  -  **상호작용**  
      
      <p align="center"><img src = "https://images.velog.io/images/cedongne/post/4d306e9c-46f8-4bb5-b6d9-3c6c4e0cbd48/image.png"></p>
      
      Inhaverse에는 회의 테이블, 강의실 문, 교탁, 책걸상 등 여러 상호작용 가능 오브젝트가 존재하며, 충분히 가까운 거리에서 해당 오브젝트를 바라보거나 마우스 커서를 올려놓으면 상호작용 외곽선과 UI가 표시된다. 이 상태에서 오브젝트를 클릭하면 각 오브젝트에 맞는 상호작용 이벤트가 발생한다. 
  - **회의**  
      <p align="center"><img src = "https://images.velog.io/images/cedongne/post/600ef826-eafe-4515-9726-9ef44e58f54f/image.png"></p>
      
     회의실은 외부와 단절된 독립 음성채널과 채팅채널을 제공한다.
     
      <p align="center"><img src = "https://images.velog.io/images/cedongne/post/d5601905-7cba-4858-98e0-79dc83f576dc/image.png"></p>

    회의에 참여한 플레이어는 누구나 `화상회의 시작`버튼을 눌러 회의 생성 이벤트를 발생시킬 수 있다. 해당 이벤트 발생시 RPC를 통해 회의에 참여한 다른 플레이어의 `화상회의 시작` 버튼이 비활성화된다. 이후 개설자가 인게임의 `회의 채널명 입력` Input Field에 채널명을 입력하고 `입력` 버튼을 누르면 현재 회의실의 모든 플레이어는 `화상회의 참여` 버튼이 활성화된다.
  - **강의**
  
    강의실 문을 클릭해 상호작용을 하면 사용자의 역할에 따라 다른 이벤트가 발생한다.

    - 교수  
     
      <p align="center"><img src = "https://images.velog.io/images/cedongne/post/41cbb4bf-d89f-4371-a0db-f6c75fb93f8f/image.png"></p>

      `강의 참여`  : DB에서 해당 교수 사용자가 개설한 강의 목록을 가져온다. 강의를 클릭하면 Classroom Scene으로 씬 전환이 이루어지며, 해당 강의 이름의 룸으로 접속할 수 있다.  
`강의 개설`  : 강의 개설 윈도우를 출력하고 입력한 정보에 맞는 새로운 강의를 개설하여 DB에 저장할 수 있다. 
    
      <p align="center"><img src = "https://images.velog.io/images/cedongne/post/70075b81-3988-4602-a3c3-9296cb62a0e1/image.png"></p>

      `강의 정보 수정` : DB에서 해당 교수 사용자가 개설한 강의 목록을 가져오고 해당 강의의 정보나 수강생 목록을 수정하여 DB에 저장할 수 있다.

    - 학생
    
      `강의 참여` : DB에서 해당 교수 사용자가 개설한 강의 목록을 가져온다. 강의를 클릭하면 Classroom Scene으로 씬 전환이 이루어지며, 해당 강의 이름의 룸으로 접속한다.

    효율적인 서버 관리를 위하여 강의 참여시 기존의 씬에서 벗어나 강의실만 존재하는 씬으로 이동하고, 강의 참여 시 DB에 저장된 강의의 강의 시각 및 날짜를 현재 시각 및 날짜와 비교하여 강의 시간과 일치하면 해당 강의실에 입장하고, 그렇지 않다면 Open Class 룸으로 입장한다.
    
  - **강의실 내부**
  
      <p align="center"><img src = "https://images.velog.io/images/cedongne/post/e99b8732-9195-4168-a1ec-056582391431/image.png"></p>

    강의실 입장에 성공하면 위와 같은 Classroom Scene으로 이동한다. 강의실 내부는 외부와 완전히 분리된 채팅, 음성 채널을 사용한다. 하단의 강의실 인원은  

    ***<p align="center">_현재 강의실에 접속 중인 학생 수 / DB에 등록된 해당 수업의 전체 학생 수_ </p>***

    를 의미한다.  
  
    교수 계정은 교탁, 학생 계정은 강의실 책상과 상호작용을 하게 되면 각 역할에 맞는 수업 이벤트가 발생한다. 학생 계정은 교탁과의 상호작용 이벤트를 일으킬 수 없다.

    - 교수  
    
      <p align="center"><img src = "https://images.velog.io/images/cedongne/post/40e082e1-c657-4f02-9881-41b493bda177/image.png"></p>
    
      `수업 시작` 버튼을 통해 Owake 브라우저에 접속하고 화상수업의 채널명을 입력하여 학생들에게 전달한다. `수업 시작` 버튼을 누른 순간부터 자동 출석이 시작되며, 강의실에 있는 모든 학생들은 출석 카운트가 5분마다 증가한다. 
      
       수업이 종료되었을 때 현재 강의실 내 모든 플레이어의 출석 카운트를 DB에 저장하고, 교수의 출석 카운트와 모든 학생의 출석 카운트를 각각 비교하여 카운트가 2 이하로 차이나면 출석으로 인정하고, 그렇지 않다면 결석 처리된다.

     - 학생  
    
        <p align="center"><img src = "https://images.velog.io/images/cedongne/post/3de94d2a-cf5c-46fa-99c5-b35d6f358f81/image.png"></p>
      
        강의실 내 좌석에 착석하고, 교수가 화상수업 채널명을 입력하면 `화상수업 참여` 버튼이 활성화되고 교수가 입력한 채널명이 출력된다. 학생은 교수가 수업을 시작한 시점부터 출석 카운팅이 시작되고, 중도에 퇴장하면 카운팅이 중단되고 다시 입장했을 때(또는 늦게 입장하였을 때) 다시 카운팅이 시작된다.

  - **부스**
    
      <p align="center"><img src = "https://images.velog.io/images/cedongne/post/7c5e6cac-08e2-45a2-bfae-162673c79769/image.png"></p>
      
    캠퍼스 내의 부스 앞에 놓인 빈 게시판에 상호작용을 하면 해당 부스의 호스트가 될 수 있다. 이미지의 Url을 입력하면 이미지가 게임 내 모든 플레이어에게 공유되고, 해당 부스에 입장해 독립된 채팅, 음성채널에서 소통이 가능하다.
    
     <p align="center"><img src = "https://images.velog.io/images/cedongne/post/bde46669-3055-4c7f-bc4f-5e2623dde2f2/image.png"></p>
      
      호스트는 다시 게시판을 클릭하면 호스팅을 해제할 수 있고, 호스트가 아닌 플레이어가 게시판을 클릭하면 해당 이미지를 큰 화면으로 조회할 수 있다.

  - **스크린샷**
    
      <p align="center"><img src = "https://images.velog.io/images/cedongne/post/6aae385f-b5d3-4c06-94cf-8e1fcc0e19db/image.png"></p>

    HUD의 Capture 버튼을 이용해 HUD를 제거한 현재 인게임 화면을 캡쳐하고, 로컬 저장소에 저장할 수 있다.  


- # 발전 계획
   - 현재 Inhaverse에서 사용 중인 화상 통신 솔루션 Owake는 전용 API를 제공하지 않아 유저친화적인 솔루션이라고 할 수 없다. 차후 Inhaverse 전용 웹 화상 통신 솔루션을 개발하고 API를 사용하여 편의성을 극대화한 시스템으로의 발전시킨다.
  - 개인 PC를 서버로 사용하고 있는 탓에 대규모의 인원을 수용하기는 어려움이 따른다. 전용 서버를 갖추어 더 많은 플레이어를 수용하고 화상 통신 솔루션에 힘을 보탬으로써 쾌적한 환경을 구축한다.
    
   - 학교 DB를 Inhaverse에 연동하였을 때 자동 출석 등 이미 시스템 내에 구현된 기능을 교수자의 역할에서 매우 편리하게 사용할 수 있고, 이후 점수 입력이나 과제 제출, 시험 등의 기능을 추가하여 GUI가 구현된 E-learning에 가깝도록 시스템을 발전시킨다.
