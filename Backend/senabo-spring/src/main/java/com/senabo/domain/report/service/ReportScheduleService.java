package com.senabo.domain.report.service;

import com.senabo.config.firebase.FCMMessage;
import com.senabo.config.firebase.FCMService;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import lombok.AllArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;

import java.util.List;

@Slf4j
@Service
@AllArgsConstructor
public class ReportScheduleService {
    private final MemberService memberService;
    private final ReportService reportService;
    private final FCMService fcmService;

    // 매일 다음 날 오전 1시 00분에 실행
    @Scheduled(cron = "0 0 1 * * *")
    public void scheduleReport() {
        log.info("리포트 스케줄러 실행");
        List<Member> allMember = memberService.findAllMemberNonComplete();
        for (Member member : allMember) {
            if(member.getId() == 16) continue; // test용 계정 제외
            reportService.scheduleReport(member);
        }
    }

    // 매일 오전 8시 00분에 실행
    @Scheduled(cron = "0 0 8 * * *")
    public void scheduleFCMReport() {
        log.info("리포트 FCM 스케줄러 실행");

        List<FCMMessage> reoprtList = memberService.findAllMember().stream()
                .filter(member -> member.getDeviceToken() != null && reportService.check7Days(member) && member.getId() != 16) // test용 계정 제외
                .map(member -> fcmService.makeMessage("세상에 나쁜 보호자는 있다", "주간 리포트를 확인해주세요!", member.getDeviceToken()))
                .toList();

        fcmService.sendFCM(reoprtList);
    }
}
